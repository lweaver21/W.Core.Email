using Microsoft.Extensions.Options;

namespace W.Core.Email.Configuration;

/// <summary>
/// Validates GmailAuthOptions configuration.
/// </summary>
public class GmailOptionsValidator : IValidateOptions<GmailAuthOptions>
{
    public ValidateOptionsResult Validate(string? name, GmailAuthOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.CredentialsPath))
        {
            failures.Add("CredentialsPath is required.");
        }

        if (string.IsNullOrWhiteSpace(options.SenderEmail))
        {
            failures.Add("SenderEmail is required.");
        }
        else if (!IsValidEmail(options.SenderEmail))
        {
            failures.Add("SenderEmail must be a valid email address.");
        }

        if (string.IsNullOrWhiteSpace(options.ApplicationName))
        {
            failures.Add("ApplicationName is required.");
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
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
