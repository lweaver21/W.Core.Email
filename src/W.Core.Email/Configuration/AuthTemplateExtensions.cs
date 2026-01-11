using W.Core.Email.Templating;

namespace W.Core.Email.Configuration;

/// <summary>
/// Extension methods for registering authentication email templates.
/// </summary>
public static class AuthTemplateExtensions
{
    /// <summary>
    /// Adds all standard authentication email templates.
    /// </summary>
    /// <param name="builder">The email service builder.</param>
    /// <returns>The builder for method chaining.</returns>
    public static EmailServiceBuilder AddAuthTemplates(this EmailServiceBuilder builder)
    {
        return builder
            .AddTemplate("EmailConfirmation", AuthTemplates.EmailConfirmation)
            .AddTemplate("PasswordReset", AuthTemplates.PasswordReset)
            .AddTemplate("TwoFactorEnabled", AuthTemplates.TwoFactorEnabled)
            .AddTemplate("NewLoginDetected", AuthTemplates.NewLoginDetected)
            .AddTemplate("AccountLocked", AuthTemplates.AccountLocked)
            .AddTemplate("PasswordChanged", AuthTemplates.PasswordChanged);
    }

    /// <summary>
    /// Adds the email confirmation template.
    /// Placeholders: {{UserName}}, {{ConfirmationLink}}, {{AppName}}, {{ExpiryHours}}
    /// </summary>
    public static EmailServiceBuilder AddEmailConfirmationTemplate(this EmailServiceBuilder builder)
    {
        return builder.AddTemplate("EmailConfirmation", AuthTemplates.EmailConfirmation);
    }

    /// <summary>
    /// Adds the password reset template.
    /// Placeholders: {{UserName}}, {{ResetLink}}, {{AppName}}, {{ExpiryMinutes}}
    /// </summary>
    public static EmailServiceBuilder AddPasswordResetTemplate(this EmailServiceBuilder builder)
    {
        return builder.AddTemplate("PasswordReset", AuthTemplates.PasswordReset);
    }

    /// <summary>
    /// Adds the two-factor authentication enabled template.
    /// Placeholders: {{UserName}}, {{AppName}}, {{EnabledAt}}
    /// </summary>
    public static EmailServiceBuilder AddTwoFactorEnabledTemplate(this EmailServiceBuilder builder)
    {
        return builder.AddTemplate("TwoFactorEnabled", AuthTemplates.TwoFactorEnabled);
    }

    /// <summary>
    /// Adds the new login detected template.
    /// Placeholders: {{UserName}}, {{AppName}}, {{LoginTime}}, {{IpAddress}}, {{Location}}, {{Device}}, {{ReportLink}}
    /// </summary>
    public static EmailServiceBuilder AddNewLoginDetectedTemplate(this EmailServiceBuilder builder)
    {
        return builder.AddTemplate("NewLoginDetected", AuthTemplates.NewLoginDetected);
    }

    /// <summary>
    /// Adds the account locked template.
    /// Placeholders: {{UserName}}, {{AppName}}, {{LockReason}}, {{UnlockLink}}
    /// </summary>
    public static EmailServiceBuilder AddAccountLockedTemplate(this EmailServiceBuilder builder)
    {
        return builder.AddTemplate("AccountLocked", AuthTemplates.AccountLocked);
    }

    /// <summary>
    /// Adds the password changed template.
    /// Placeholders: {{UserName}}, {{AppName}}, {{ChangedAt}}, {{ReportLink}}
    /// </summary>
    public static EmailServiceBuilder AddPasswordChangedTemplate(this EmailServiceBuilder builder)
    {
        return builder.AddTemplate("PasswordChanged", AuthTemplates.PasswordChanged);
    }
}
