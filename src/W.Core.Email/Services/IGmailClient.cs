using W.Core.Email.Models;

namespace W.Core.Email.Services;

/// <summary>
/// Low-level client for Gmail API operations.
/// </summary>
public interface IGmailClient
{
    /// <summary>
    /// Sends an email message.
    /// </summary>
    /// <param name="message">The email message to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The message ID assigned by Gmail.</returns>
    Task<string> SendAsync(EmailMessage message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current user's email profile information.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The user's email address.</returns>
    Task<string> GetUserEmailAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the client is properly authenticated.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if authenticated, false otherwise.</returns>
    Task<bool> IsAuthenticatedAsync(CancellationToken cancellationToken = default);
}
