namespace W.Core.Email.Exceptions;

/// <summary>
/// Exception thrown when rate limits are exceeded.
/// </summary>
public class RateLimitException : EmailException
{
    /// <summary>
    /// Time to wait before retrying, if provided by the server.
    /// </summary>
    public TimeSpan? RetryAfter { get; }

    /// <summary>
    /// The quota that was exceeded, if available.
    /// </summary>
    public int? QuotaLimit { get; }

    /// <summary>
    /// The current usage when the limit was hit.
    /// </summary>
    public int? CurrentUsage { get; }

    /// <summary>
    /// Creates a new RateLimitException.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="retryAfter">Time to wait before retrying.</param>
    public RateLimitException(string message, TimeSpan? retryAfter = null)
        : base(message, EmailErrorCode.RateLimitExceeded)
    {
        RetryAfter = retryAfter;
    }

    /// <summary>
    /// Creates a new RateLimitException with quota information.
    /// </summary>
    /// <param name="message">The error message.</param>
    /// <param name="quotaLimit">The quota limit.</param>
    /// <param name="currentUsage">The current usage.</param>
    /// <param name="retryAfter">Time to wait before retrying.</param>
    public RateLimitException(string message, int quotaLimit, int currentUsage, TimeSpan? retryAfter = null)
        : base(message, EmailErrorCode.RateLimitExceeded)
    {
        QuotaLimit = quotaLimit;
        CurrentUsage = currentUsage;
        RetryAfter = retryAfter;
    }

    /// <summary>
    /// Creates an exception for daily quota exceeded.
    /// </summary>
    public static RateLimitException DailyQuotaExceeded(int limit)
        => new($"Daily sending quota of {limit} emails exceeded.", EmailErrorCode.QuotaExceeded);

    /// <summary>
    /// Creates an exception for per-second rate limit.
    /// </summary>
    public static RateLimitException PerSecondLimitExceeded(TimeSpan retryAfter)
        => new("Rate limit exceeded. Too many requests per second.", retryAfter);

    private RateLimitException(string message, EmailErrorCode code)
        : base(message, code)
    {
    }
}
