namespace W.Core.Email.Exceptions;

/// <summary>
/// Base exception for all email-related errors.
/// </summary>
public class EmailException : Exception
{
    /// <summary>
    /// The error code identifying the type of error.
    /// </summary>
    public EmailErrorCode ErrorCode { get; }

    /// <summary>
    /// Creates a new EmailException.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="errorCode">The error code.</param>
    public EmailException(string message, EmailErrorCode errorCode = EmailErrorCode.Unknown)
        : base(message)
    {
        ErrorCode = errorCode;
    }

    /// <summary>
    /// Creates a new EmailException with an inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="errorCode">The error code.</param>
    public EmailException(string message, Exception innerException, EmailErrorCode errorCode = EmailErrorCode.Unknown)
        : base(message, innerException)
    {
        ErrorCode = errorCode;
    }
}
