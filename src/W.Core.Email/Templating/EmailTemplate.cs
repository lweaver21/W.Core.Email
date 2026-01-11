using W.Core.Email.Models;

namespace W.Core.Email.Templating;

/// <summary>
/// Default implementation of an email template.
/// </summary>
public class EmailTemplate : IEmailTemplate
{
    /// <inheritdoc />
    public required string Subject { get; init; }

    /// <inheritdoc />
    public required string BodyTemplate { get; init; }

    /// <inheritdoc />
    public bool IsHtml { get; init; } = true;

    /// <inheritdoc />
    public EmailPriority DefaultPriority { get; init; } = EmailPriority.Normal;

    /// <summary>
    /// Creates a simple text email template.
    /// </summary>
    /// <param name="subject">The subject template.</param>
    /// <param name="body">The body template.</param>
    /// <returns>A new EmailTemplate instance.</returns>
    public static EmailTemplate Text(string subject, string body) => new()
    {
        Subject = subject,
        BodyTemplate = body,
        IsHtml = false
    };

    /// <summary>
    /// Creates an HTML email template.
    /// </summary>
    /// <param name="subject">The subject template.</param>
    /// <param name="body">The HTML body template.</param>
    /// <returns>A new EmailTemplate instance.</returns>
    public static EmailTemplate Html(string subject, string body) => new()
    {
        Subject = subject,
        BodyTemplate = body,
        IsHtml = true
    };
}
