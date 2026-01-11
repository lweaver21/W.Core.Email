using System.Text;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using W.Core.Email.Configuration;
using W.Core.Email.Exceptions;
using W.Core.Email.Internal;
using W.Core.Email.Models;

namespace W.Core.Email.Services;

/// <summary>
/// Gmail API client implementation.
/// </summary>
public class GmailClient : IGmailClient, IDisposable
{
    private readonly GmailAuthOptions _options;
    private readonly ILogger<GmailClient> _logger;
    private GmailService? _service;
    private bool _disposed;

    private static readonly string[] Scopes = { GmailService.Scope.GmailSend, GmailService.Scope.GmailReadonly };

    public GmailClient(
        IOptions<GmailAuthOptions> options,
        ILogger<GmailClient> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task<string> SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var service = await GetServiceAsync(cancellationToken);

        try
        {
            // Build MIME message
            var mimeMessage = MimeMessageBuilder.Build(message, _options.SenderEmail);
            var rawMessage = MimeMessageBuilder.ToBase64UrlString(mimeMessage);

            var gmailMessage = new Message { Raw = rawMessage };

            _logger.LogDebug("Sending email to {Recipients}", string.Join(", ", message.To));

            var request = service.Users.Messages.Send(gmailMessage, "me");
            var response = await ExecuteWithRetryAsync(
                () => request.ExecuteAsync(cancellationToken),
                cancellationToken);

            _logger.LogInformation("Email sent successfully. MessageId: {MessageId}", response.Id);

            return response.Id;
        }
        catch (Google.GoogleApiException ex) when ((int)ex.HttpStatusCode == 429) // TooManyRequests
        {
            _logger.LogWarning("Gmail rate limit exceeded");
            throw new RateLimitException("Gmail API rate limit exceeded. Please try again later.",
                TimeSpan.FromSeconds(60));
        }
        catch (Google.GoogleApiException ex)
        {
            _logger.LogError(ex, "Gmail API error: {Message}", ex.Message);
            throw new SendFailedException($"Failed to send email via Gmail: {ex.Message}", ex);
        }
    }

    /// <inheritdoc />
    public async Task<string> GetUserEmailAsync(CancellationToken cancellationToken = default)
    {
        var service = await GetServiceAsync(cancellationToken);

        try
        {
            var profile = await service.Users.GetProfile("me").ExecuteAsync(cancellationToken);
            return profile.EmailAddress;
        }
        catch (Google.GoogleApiException ex)
        {
            _logger.LogError(ex, "Failed to get user profile");
            throw new AuthenticationException("Failed to get user profile from Gmail", ex);
        }
    }

    /// <inheritdoc />
    public async Task<bool> IsAuthenticatedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var service = await GetServiceAsync(cancellationToken);
            await service.Users.GetProfile("me").ExecuteAsync(cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<GmailService> GetServiceAsync(CancellationToken cancellationToken)
    {
        if (_service is not null)
        {
            return _service;
        }

        var credential = await GetCredentialAsync(cancellationToken);

        _service = new GmailService(new BaseClientService.Initializer
        {
            HttpClientInitializer = credential,
            ApplicationName = _options.ApplicationName
        });

        return _service;
    }

    private async Task<ICredential> GetCredentialAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(_options.CredentialsPath))
        {
            throw AuthenticationException.CredentialsFileNotFound(_options.CredentialsPath);
        }

        try
        {
            if (_options.AuthMethod == GmailAuthMethod.ServiceAccount)
            {
                return await GetServiceAccountCredentialAsync(cancellationToken);
            }

            return await GetOAuthCredentialAsync(cancellationToken);
        }
        catch (Exception ex) when (ex is not AuthenticationException)
        {
            _logger.LogError(ex, "Failed to authenticate with Gmail");
            throw new AuthenticationException("Failed to authenticate with Gmail API", ex,
                _options.AuthMethod.ToString());
        }
    }

    private Task<ICredential> GetServiceAccountCredentialAsync(CancellationToken cancellationToken)
    {
        using var stream = File.OpenRead(_options.CredentialsPath);
        var credential = GoogleCredential.FromStream(stream)
            .CreateScoped(Scopes)
            .CreateWithUser(_options.SenderEmail);

        var result = credential.UnderlyingCredential as ICredential
            ?? throw new AuthenticationException("Failed to create service account credential", "ServiceAccount");

        return Task.FromResult(result);
    }

    private async Task<ICredential> GetOAuthCredentialAsync(CancellationToken cancellationToken)
    {
        using var stream = File.OpenRead(_options.CredentialsPath);
        var secrets = await GoogleClientSecrets.FromStreamAsync(stream, cancellationToken);

        var tokenPath = _options.TokenStorePath ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "W.Core.Email",
            "tokens");

        var credential = await GoogleWebAuthorizationBroker.AuthorizeAsync(
            secrets.Secrets,
            Scopes,
            "user",
            cancellationToken,
            new Google.Apis.Util.Store.FileDataStore(tokenPath, true));

        return credential;
    }

    private async Task<T> ExecuteWithRetryAsync<T>(
        Func<Task<T>> action,
        CancellationToken cancellationToken,
        int maxRetries = 3)
    {
        var delay = TimeSpan.FromSeconds(1);

        for (var attempt = 0; attempt < maxRetries; attempt++)
        {
            try
            {
                return await action();
            }
            catch (Google.GoogleApiException ex) when (
                ex.HttpStatusCode == System.Net.HttpStatusCode.ServiceUnavailable ||
                ex.HttpStatusCode == System.Net.HttpStatusCode.GatewayTimeout)
            {
                if (attempt == maxRetries - 1)
                {
                    throw;
                }

                _logger.LogWarning("Gmail API returned {StatusCode}, retrying in {Delay}...",
                    ex.HttpStatusCode, delay);

                await Task.Delay(delay, cancellationToken);
                delay = TimeSpan.FromTicks(delay.Ticks * 2); // Exponential backoff
            }
        }

        throw new InvalidOperationException("Retry loop exited unexpectedly");
    }

    public void Dispose()
    {
        if (_disposed) return;
        _service?.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }
}
