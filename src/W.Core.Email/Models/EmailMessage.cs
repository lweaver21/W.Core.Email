namespace W.Core.Email.Models;

/// <summary>
/// Represents a complete email message ready to be sent.
/// </summary>
public class EmailMessage
{
    /// <summary>
    /// The sender email address.
    /// </summary>
    public string? From { get; set; }

    /// <summary>
    /// The sender display name.
    /// </summary>
    public string? FromName { get; set; }

    /// <summary>
    /// Primary recipient email addresses.
    /// </summary>
    public ICollection<string> To { get; set; } = new List<string>();

    /// <summary>
    /// Carbon copy recipient email addresses.
    /// </summary>
    public ICollection<string> Cc { get; set; } = new List<string>();

    /// <summary>
    /// Blind carbon copy recipient email addresses.
    /// </summary>
    public ICollection<string> Bcc { get; set; } = new List<string>();

    /// <summary>
    /// Reply-to email address.
    /// </summary>
    public string? ReplyTo { get; set; }

    /// <summary>
    /// Email subject line.
    /// </summary>
    public required string Subject { get; set; }

    /// <summary>
    /// Email body content.
    /// </summary>
    public required string Body { get; set; }

    /// <summary>
    /// Whether the body is HTML formatted.
    /// </summary>
    public bool IsHtml { get; set; } = false;

    /// <summary>
    /// Email priority level.
    /// </summary>
    public EmailPriority Priority { get; set; } = EmailPriority.Normal;

    /// <summary>
    /// File attachments.
    /// </summary>
    public ICollection<EmailAttachment> Attachments { get; set; } = new List<EmailAttachment>();

    /// <summary>
    /// Custom headers to include in the email.
    /// </summary>
    public IDictionary<string, string> CustomHeaders { get; set; } = new Dictionary<string, string>();

    /// <summary>
    /// Creates an EmailMessage with a single recipient.
    /// </summary>
    public static EmailMessage Create(string to, string subject, string body, bool isHtml = false)
    {
        return new EmailMessage
        {
            To = new List<string> { to },
            Subject = subject,
            Body = body,
            IsHtml = isHtml
        };
    }

    /// <summary>
    /// Creates an EmailMessage with multiple recipients.
    /// </summary>
    public static EmailMessage Create(IEnumerable<string> to, string subject, string body, bool isHtml = false)
    {
        return new EmailMessage
        {
            To = to.ToList(),
            Subject = subject,
            Body = body,
            IsHtml = isHtml
        };
    }
}
