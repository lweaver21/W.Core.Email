namespace W.Core.Email.Exceptions;

/// <summary>
/// Exception thrown when sending an email fails.
/// </summary>
public class SendFailedException : EmailException
{
    /// <summary>
    /// The recipient email addresses that failed.
    /// </summary>
    public IReadOnlyList<string>? FailedRecipients { get; }

    /// <summary>
    /// The Gmail API error code, if available.
    /// </summary>
    public int? GmailErrorCode { get; }

    /// <summary>
    /// Creates a new SendFailedException.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorCode">The error code.</param>
    public SendFailedException(string message, EmailErrorCode errorCode = EmailErrorCode.SendFailed)
        : base(message, errorCode)
    {
    }

    /// <summary>
    /// Creates a new SendFailedException with an inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="errorCode">The error code.</param>
    public SendFailedException(string message, Exception innerException, EmailErrorCode errorCode = EmailErrorCode.SendFailed)
        : base(message, innerException, errorCode)
    {
    }

    /// <summary>
    /// Creates a new SendFailedException with failed recipients.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="failedRecipients">The recipients that failed.</param>
    public SendFailedException(string message, IEnumerable<string> failedRecipients)
        : base(message, EmailErrorCode.InvalidRecipient)
    {
        FailedRecipients = failedRecipients.ToList().AsReadOnly();
    }

    /// <summary>
    /// Creates an exception for invalid recipient.
    /// </summary>
    public static SendFailedException InvalidRecipient(string email)
        => new($"Invalid recipient email address: {email}", EmailErrorCode.InvalidRecipient);

    /// <summary>
    /// Creates an exception for invalid sender.
    /// </summary>
    public static SendFailedException InvalidSender(string email)
        => new($"Invalid or unauthorized sender email address: {email}", EmailErrorCode.InvalidSender);

    /// <summary>
    /// Creates an exception for oversized attachment.
    /// </summary>
    public static SendFailedException AttachmentTooLarge(string fileName, long size, long maxSize)
        => new($"Attachment '{fileName}' ({size} bytes) exceeds maximum size of {maxSize} bytes.", EmailErrorCode.AttachmentTooLarge);

    /// <summary>
    /// Creates an exception for oversized message.
    /// </summary>
    public static SendFailedException MessageTooLarge(long size, long maxSize)
        => new($"Message ({size} bytes) exceeds maximum size of {maxSize} bytes.", EmailErrorCode.MessageTooLarge);
}
