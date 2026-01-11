namespace W.Core.Email.Models;

/// <summary>
/// Options for customizing individual email sends.
/// These override default settings configured in EmailOptions.
/// </summary>
public class SendEmailOptions
{
    /// <summary>
    /// Override the email priority for this send.
    /// </summary>
    public EmailPriority? Priority { get; set; }

    /// <summary>
    /// Additional attachments for this email.
    /// </summary>
    public IEnumerable<EmailAttachment>? Attachments { get; set; }

    /// <summary>
    /// Override the reply-to address for this email.
    /// </summary>
    public string? ReplyTo { get; set; }

    /// <summary>
    /// Carbon copy recipients for this email.
    /// </summary>
    public IEnumerable<string>? Cc { get; set; }

    /// <summary>
    /// Blind carbon copy recipients for this email.
    /// </summary>
    public IEnumerable<string>? Bcc { get; set; }

    /// <summary>
    /// Custom headers to add to this email.
    /// </summary>
    public IDictionary<string, string>? CustomHeaders { get; set; }

    /// <summary>
    /// Whether to track when this email is opened.
    /// </summary>
    public bool? TrackOpens { get; set; }

    /// <summary>
    /// Whether to track when links in this email are clicked.
    /// </summary>
    public bool? TrackClicks { get; set; }

    /// <summary>
    /// Override the sender name for this email.
    /// </summary>
    public string? FromName { get; set; }

    /// <summary>
    /// Override the sender email address for this email (must be authorized).
    /// </summary>
    public string? FromEmail { get; set; }

    /// <summary>
    /// Creates a new SendEmailOptions with high priority.
    /// </summary>
    public static SendEmailOptions HighPriority() => new() { Priority = EmailPriority.High };

    /// <summary>
    /// Creates a new SendEmailOptions with urgent priority.
    /// </summary>
    public static SendEmailOptions Urgent() => new() { Priority = EmailPriority.Urgent };

    /// <summary>
    /// Creates a new SendEmailOptions with specified attachments.
    /// </summary>
    public static SendEmailOptions WithAttachments(params EmailAttachment[] attachments) =>
        new() { Attachments = attachments };

    /// <summary>
    /// Creates a new SendEmailOptions with CC recipients.
    /// </summary>
    public static SendEmailOptions WithCc(params string[] recipients) =>
        new() { Cc = recipients };
}
