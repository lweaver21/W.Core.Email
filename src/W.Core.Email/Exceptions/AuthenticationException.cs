namespace W.Core.Email.Exceptions;

/// <summary>
/// Exception thrown when authentication with the email provider fails.
/// </summary>
public class AuthenticationException : EmailException
{
    /// <summary>
    /// The authentication method that was attempted.
    /// </summary>
    public string? AuthMethod { get; }

    /// <summary>
    /// Creates a new AuthenticationException.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="authMethod">The authentication method attempted.</param>
    public AuthenticationException(string message, string? authMethod = null)
        : base(message, EmailErrorCode.AuthenticationFailed)
    {
        AuthMethod = authMethod;
    }

    /// <summary>
    /// Creates a new AuthenticationException with an inner exception.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The inner exception.</param>
    /// <param name="authMethod">The authentication method attempted.</param>
    public AuthenticationException(string message, Exception innerException, string? authMethod = null)
        : base(message, innerException, EmailErrorCode.AuthenticationFailed)
    {
        AuthMethod = authMethod;
    }

    /// <summary>
    /// Creates an exception for invalid credentials.
    /// </summary>
    public static AuthenticationException InvalidCredentials(string details)
        => new($"Invalid credentials: {details}") { };

    /// <summary>
    /// Creates an exception for a missing credentials file.
    /// </summary>
    public static AuthenticationException CredentialsFileNotFound(string path)
        => new($"Credentials file not found: {path}");

    /// <summary>
    /// Creates an exception for expired OAuth token.
    /// </summary>
    public static AuthenticationException TokenExpired()
        => new("OAuth token has expired and refresh failed.", "OAuth");
}
