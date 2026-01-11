namespace W.Core.Email.Models;

/// <summary>
/// Email priority levels.
/// </summary>
public enum EmailPriority
{
    /// <summary>
    /// Low priority email.
    /// </summary>
    Low = 0,

    /// <summary>
    /// Normal priority email (default).
    /// </summary>
    Normal = 1,

    /// <summary>
    /// High priority email.
    /// </summary>
    High = 2,

    /// <summary>
    /// Urgent priority email.
    /// </summary>
    Urgent = 3
}
