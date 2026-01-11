using System.ComponentModel.DataAnnotations;

namespace W.Core.Email.Configuration;

/// <summary>
/// Configuration options for the email service.
/// </summary>
public class EmailOptions
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "Email";

    /// <summary>
    /// Unique identifier for the project using this email service.
    /// Used to isolate templates between different consuming projects.
    /// </summary>
    [Required(ErrorMessage = "ProjectKey is required")]
    public string ProjectKey { get; set; } = string.Empty;

    /// <summary>
    /// Default sender display name for outgoing emails.
    /// </summary>
    public string? DefaultSenderName { get; set; }

    /// <summary>
    /// Default sender email address. Can be overridden by Gmail provider settings.
    /// </summary>
    public string? DefaultSenderEmail { get; set; }

    /// <summary>
    /// Default reply-to email address.
    /// </summary>
    public string? DefaultReplyTo { get; set; }

    /// <summary>
    /// Whether to track email opens by default.
    /// </summary>
    public bool TrackOpensByDefault { get; set; } = false;

    /// <summary>
    /// Whether to track link clicks by default.
    /// </summary>
    public bool TrackClicksByDefault { get; set; } = false;
}
