using System.ComponentModel.DataAnnotations;

namespace W.Core.Email.Configuration;

/// <summary>
/// Authentication options for Gmail API.
/// </summary>
public class GmailAuthOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "Email:Gmail";

    /// <summary>
    /// Path to the OAuth credentials JSON file or Service Account key file.
    /// </summary>
    [Required(ErrorMessage = "CredentialsPath is required")]
    public string CredentialsPath { get; set; } = string.Empty;

    /// <summary>
    /// The email address to send emails from.
    /// For Service Accounts, this must be a user in the domain with delegation.
    /// </summary>
    [Required(ErrorMessage = "SenderEmail is required")]
    [EmailAddress(ErrorMessage = "SenderEmail must be a valid email address")]
    public string SenderEmail { get; set; } = string.Empty;

    /// <summary>
    /// The authentication method to use.
    /// </summary>
    public GmailAuthMethod AuthMethod { get; set; } = GmailAuthMethod.OAuth;

    /// <summary>
    /// Application name shown in Gmail API requests.
    /// </summary>
    public string ApplicationName { get; set; } = "W.Core.Email";

    /// <summary>
    /// Path to store OAuth tokens. Only used with OAuth authentication.
    /// </summary>
    public string? TokenStorePath { get; set; }
}

/// <summary>
/// Gmail API authentication methods.
/// </summary>
public enum GmailAuthMethod
{
    /// <summary>
    /// OAuth 2.0 with user consent flow.
    /// </summary>
    OAuth,

    /// <summary>
    /// Service Account with domain-wide delegation.
    /// </summary>
    ServiceAccount
}
