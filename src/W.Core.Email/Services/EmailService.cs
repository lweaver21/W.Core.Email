using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using W.Core.Email.Configuration;
using W.Core.Email.Exceptions;
using W.Core.Email.Models;
using W.Core.Email.Templating;

namespace W.Core.Email.Services;

/// <summary>
/// High-level email service implementation with template support.
/// </summary>
public class EmailService : IEmailService
{
    private readonly IGmailClient _gmailClient;
    private readonly ITemplateRegistry _templateRegistry;
    private readonly TemplateRenderer _templateRenderer;
    private readonly EmailOptions _options;
    private readonly ILogger<EmailService> _logger;

    public EmailService(
        IGmailClient gmailClient,
        ITemplateRegistry templateRegistry,
        TemplateRenderer templateRenderer,
        IOptions<EmailOptions> options,
        ILogger<EmailService> logger)
    {
        _gmailClient = gmailClient;
        _templateRegistry = templateRegistry;
        _templateRenderer = templateRenderer;
        _options = options.Value;
        _logger = logger;
    }

    /// <inheritdoc />
    public Task<SendResult> SendAsync(
        string templateType,
        string to,
        object? model = null,
        SendEmailOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        return SendAsync(templateType, new[] { to }, model, options, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<SendResult> SendAsync(
        string templateType,
        IEnumerable<string> to,
        object? model = null,
        SendEmailOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Get template
            if (!_templateRegistry.TryGet(_options.ProjectKey, templateType, out var template))
            {
                _logger.LogError("Template '{TemplateType}' not found for project '{ProjectKey}'",
                    templateType, _options.ProjectKey);
                throw new TemplateNotFoundException(_options.ProjectKey, templateType);
            }

            // Render template
            var (subject, body) = _templateRenderer.RenderTemplate(template!, model);

            // Build email message
            var message = BuildMessage(to, subject, body, template!, options);

            // Send via Gmail
            var messageId = await _gmailClient.SendAsync(message, cancellationToken);

            _logger.LogInformation(
                "Email sent successfully using template '{TemplateType}' to {Recipients}. MessageId: {MessageId}",
                templateType, string.Join(", ", to), messageId);

            return SendResult.Succeeded(messageId);
        }
        catch (TemplateNotFoundException)
        {
            throw;
        }
        catch (EmailException ex)
        {
            _logger.LogError(ex, "Failed to send email using template '{TemplateType}'", templateType);
            return SendResult.Failed(ex.Message, ex.ErrorCode.ToString(), ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending email using template '{TemplateType}'", templateType);
            return SendResult.FromException(ex);
        }
    }

    /// <inheritdoc />
    public async Task<SendResult> SendRawAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Apply defaults if not set
            message.From ??= _options.DefaultSenderEmail;
            message.FromName ??= _options.DefaultSenderName;
            message.ReplyTo ??= _options.DefaultReplyTo;

            var messageId = await _gmailClient.SendAsync(message, cancellationToken);

            _logger.LogInformation(
                "Raw email sent successfully to {Recipients}. MessageId: {MessageId}",
                string.Join(", ", message.To), messageId);

            return SendResult.Succeeded(messageId);
        }
        catch (EmailException ex)
        {
            _logger.LogError(ex, "Failed to send raw email");
            return SendResult.Failed(ex.Message, ex.ErrorCode.ToString(), ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error sending raw email");
            return SendResult.FromException(ex);
        }
    }

    private EmailMessage BuildMessage(
        IEnumerable<string> to,
        string subject,
        string body,
        IEmailTemplate template,
        SendEmailOptions? options)
    {
        var message = new EmailMessage
        {
            Subject = subject,
            Body = body,
            IsHtml = template.IsHtml,
            Priority = options?.Priority ?? template.DefaultPriority,
            From = options?.FromEmail ?? _options.DefaultSenderEmail,
            FromName = options?.FromName ?? _options.DefaultSenderName,
            ReplyTo = options?.ReplyTo ?? _options.DefaultReplyTo
        };

        foreach (var recipient in to)
        {
            message.To.Add(recipient);
        }

        // Add CC recipients
        if (options?.Cc is not null)
        {
            foreach (var cc in options.Cc)
            {
                message.Cc.Add(cc);
            }
        }

        // Add BCC recipients
        if (options?.Bcc is not null)
        {
            foreach (var bcc in options.Bcc)
            {
                message.Bcc.Add(bcc);
            }
        }

        // Add attachments
        if (options?.Attachments is not null)
        {
            foreach (var attachment in options.Attachments)
            {
                message.Attachments.Add(attachment);
            }
        }

        // Add custom headers
        if (options?.CustomHeaders is not null)
        {
            foreach (var header in options.CustomHeaders)
            {
                message.CustomHeaders[header.Key] = header.Value;
            }
        }

        return message;
    }
}
