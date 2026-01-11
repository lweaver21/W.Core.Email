using W.Core.Email.Models;

namespace W.Core.Email;

/// <summary>
/// High-level service for sending templated emails.
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Sends an email using a registered template.
    /// </summary>
    /// <param name="templateType">The template type to use.</param>
    /// <param name="to">The recipient email address.</param>
    /// <param name="model">The model to render the template with.</param>
    /// <param name="options">Optional send options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the send operation.</returns>
    Task<SendResult> SendAsync(
        string templateType,
        string to,
        object? model = null,
        SendEmailOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends an email using a registered template to multiple recipients.
    /// </summary>
    /// <param name="templateType">The template type to use.</param>
    /// <param name="to">The recipient email addresses.</param>
    /// <param name="model">The model to render the template with.</param>
    /// <param name="options">Optional send options.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the send operation.</returns>
    Task<SendResult> SendAsync(
        string templateType,
        IEnumerable<string> to,
        object? model = null,
        SendEmailOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a raw email message without using templates.
    /// </summary>
    /// <param name="message">The email message to send.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the send operation.</returns>
    Task<SendResult> SendRawAsync(
        EmailMessage message,
        CancellationToken cancellationToken = default);
}
