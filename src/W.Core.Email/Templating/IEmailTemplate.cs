using W.Core.Email.Models;

namespace W.Core.Email.Templating;

/// <summary>
/// Represents an email template that can be rendered with a model.
/// </summary>
public interface IEmailTemplate
{
    /// <summary>
    /// The email subject template. Supports {{placeholder}} syntax.
    /// </summary>
    string Subject { get; }

    /// <summary>
    /// The email body template. Supports {{placeholder}} syntax.
    /// </summary>
    string BodyTemplate { get; }

    /// <summary>
    /// Whether the body is HTML formatted.
    /// </summary>
    bool IsHtml { get; }

    /// <summary>
    /// The default priority for emails using this template.
    /// </summary>
    EmailPriority DefaultPriority { get; }
}
