namespace W.Core.Email.Exceptions;

/// <summary>
/// Error codes for email operations.
/// </summary>
public enum EmailErrorCode
{
    /// <summary>
    /// Unknown error.
    /// </summary>
    Unknown = 0,

    /// <summary>
    /// Template was not found in the registry.
    /// </summary>
    TemplateNotFound = 1000,

    /// <summary>
    /// Template rendering failed.
    /// </summary>
    TemplateRenderFailed = 1001,

    /// <summary>
    /// Authentication with the email provider failed.
    /// </summary>
    AuthenticationFailed = 2000,

    /// <summary>
    /// OAuth credentials are invalid or expired.
    /// </summary>
    InvalidCredentials = 2001,

    /// <summary>
    /// Service account configuration is invalid.
    /// </summary>
    InvalidServiceAccount = 2002,

    /// <summary>
    /// Rate limit exceeded.
    /// </summary>
    RateLimitExceeded = 3000,

    /// <summary>
    /// Daily sending quota exceeded.
    /// </summary>
    QuotaExceeded = 3001,

    /// <summary>
    /// Email send failed.
    /// </summary>
    SendFailed = 4000,

    /// <summary>
    /// Invalid recipient email address.
    /// </summary>
    InvalidRecipient = 4001,

    /// <summary>
    /// Invalid sender email address.
    /// </summary>
    InvalidSender = 4002,

    /// <summary>
    /// Attachment is too large.
    /// </summary>
    AttachmentTooLarge = 4003,

    /// <summary>
    /// Message is too large.
    /// </summary>
    MessageTooLarge = 4004,

    /// <summary>
    /// Configuration is invalid.
    /// </summary>
    InvalidConfiguration = 5000,

    /// <summary>
    /// Credentials file not found.
    /// </summary>
    CredentialsFileNotFound = 5001
}
