using Microsoft.Extensions.Options;

namespace W.Core.Email.Configuration;

/// <summary>
/// Validates EmailOptions configuration.
/// </summary>
public class EmailOptionsValidator : IValidateOptions<EmailOptions>
{
    public ValidateOptionsResult Validate(string? name, EmailOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.ProjectKey))
        {
            failures.Add("ProjectKey is required and cannot be empty.");
        }
        else if (options.ProjectKey.Length > 50)
        {
            failures.Add("ProjectKey cannot exceed 50 characters.");
        }
        else if (!IsValidProjectKey(options.ProjectKey))
        {
            failures.Add("ProjectKey can only contain letters, numbers, hyphens, and underscores.");
        }

        if (options.DefaultSenderEmail is not null && !IsValidEmail(options.DefaultSenderEmail))
        {
            failures.Add("DefaultSenderEmail must be a valid email address.");
        }

        if (options.DefaultReplyTo is not null && !IsValidEmail(options.DefaultReplyTo))
        {
            failures.Add("DefaultReplyTo must be a valid email address.");
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }

    private static bool IsValidProjectKey(string projectKey)
    {
        return projectKey.All(c => char.IsLetterOrDigit(c) || c == '-' || c == '_');
    }

    private static bool IsValidEmail(string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}
