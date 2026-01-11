namespace W.Core.Email.Models;

/// <summary>
/// Result of an email send operation.
/// </summary>
public class SendResult
{
    /// <summary>
    /// Whether the email was sent successfully.
    /// </summary>
    public bool Success { get; private init; }

    /// <summary>
    /// The message ID assigned by the email provider (e.g., Gmail).
    /// </summary>
    public string? MessageId { get; private init; }

    /// <summary>
    /// Timestamp when the email was sent.
    /// </summary>
    public DateTimeOffset? SentAt { get; private init; }

    /// <summary>
    /// Error message if the send failed.
    /// </summary>
    public string? ErrorMessage { get; private init; }

    /// <summary>
    /// Error code if the send failed.
    /// </summary>
    public string? ErrorCode { get; private init; }

    /// <summary>
    /// The exception that caused the failure, if any.
    /// </summary>
    public Exception? Exception { get; private init; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <param name="messageId">The message ID from the provider.</param>
    /// <returns>A successful SendResult.</returns>
    public static SendResult Succeeded(string messageId)
    {
        return new SendResult
        {
            Success = true,
            MessageId = messageId,
            SentAt = DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    /// <param name="errorMessage">Description of the error.</param>
    /// <param name="errorCode">Optional error code.</param>
    /// <param name="exception">Optional exception that caused the failure.</param>
    /// <returns>A failed SendResult.</returns>
    public static SendResult Failed(string errorMessage, string? errorCode = null, Exception? exception = null)
    {
        return new SendResult
        {
            Success = false,
            ErrorMessage = errorMessage,
            ErrorCode = errorCode,
            Exception = exception
        };
    }

    /// <summary>
    /// Creates a failed result from an exception.
    /// </summary>
    /// <param name="exception">The exception that caused the failure.</param>
    /// <returns>A failed SendResult.</returns>
    public static SendResult FromException(Exception exception)
    {
        return new SendResult
        {
            Success = false,
            ErrorMessage = exception.Message,
            ErrorCode = exception.GetType().Name,
            Exception = exception
        };
    }

    /// <summary>
    /// Throws an exception if the result indicates failure.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when Success is false.</exception>
    public void EnsureSuccess()
    {
        if (!Success)
        {
            throw Exception ?? new InvalidOperationException(ErrorMessage ?? "Email send failed");
        }
    }
}
