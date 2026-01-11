using W.Core.Email.Models;

namespace W.Core.Email.Templating;

/// <summary>
/// Pre-built email templates for authentication flows.
/// </summary>
public static class AuthTemplates
{
    /// <summary>
    /// Template for email confirmation/verification emails.
    /// Placeholders: {{UserName}}, {{ConfirmationLink}}, {{AppName}}, {{ExpiryHours}}
    /// </summary>
    public static IEmailTemplate EmailConfirmation => new EmailTemplate
    {
        Subject = "Confirm your email address",
        BodyTemplate = new HtmlEmailBuilder()
            .WithTextHeader("Confirm Your Email")
            .WithBody($@"
                <p>Hello {{{{UserName}}}},</p>
                <p>Thank you for registering with {{{{AppName}}}}. Please confirm your email address by clicking the button below:</p>
                {new HtmlEmailBuilder().CreateButton("Confirm Email", "{{ConfirmationLink}}")}
                <p>Or copy and paste this link into your browser:</p>
                <p style=""word-break: break-all; color: #666666; font-size: 14px;"">{{{{ConfirmationLink}}}}</p>
                <p>This link will expire in {{{{ExpiryHours}}}} hours.</p>
                <p>If you didn't create an account with us, you can safely ignore this email.</p>
            ")
            .WithSimpleFooter("{{AppName}}")
            .Build(),
        IsHtml = true,
        DefaultPriority = EmailPriority.Normal
    };

    /// <summary>
    /// Template for password reset emails.
    /// Placeholders: {{UserName}}, {{ResetLink}}, {{AppName}}, {{ExpiryMinutes}}
    /// </summary>
    public static IEmailTemplate PasswordReset => new EmailTemplate
    {
        Subject = "Reset your password",
        BodyTemplate = new HtmlEmailBuilder()
            .WithPrimaryColor("#e74c3c")
            .WithTextHeader("Password Reset Request")
            .WithBody($@"
                <p>Hello {{{{UserName}}}},</p>
                <p>We received a request to reset your password for your {{{{AppName}}}} account.</p>
                {new HtmlEmailBuilder().WithPrimaryColor("#e74c3c").CreateButton("Reset Password", "{{ResetLink}}", "#e74c3c")}
                <p>Or copy and paste this link into your browser:</p>
                <p style=""word-break: break-all; color: #666666; font-size: 14px;"">{{{{ResetLink}}}}</p>
                <p><strong>This link will expire in {{{{ExpiryMinutes}}}} minutes.</strong></p>
                {new HtmlEmailBuilder().CreateDivider()}
                <p style=""color: #e74c3c; font-size: 14px;"">⚠️ If you didn't request a password reset, please ignore this email or contact support if you're concerned about your account security.</p>
            ")
            .WithSimpleFooter("{{AppName}}")
            .Build(),
        IsHtml = true,
        DefaultPriority = EmailPriority.High
    };

    /// <summary>
    /// Template for two-factor authentication enabled notification.
    /// Placeholders: {{UserName}}, {{AppName}}, {{EnabledAt}}
    /// </summary>
    public static IEmailTemplate TwoFactorEnabled => new EmailTemplate
    {
        Subject = "Two-factor authentication enabled",
        BodyTemplate = new HtmlEmailBuilder()
            .WithPrimaryColor("#27ae60")
            .WithTextHeader("2FA Enabled ✓")
            .WithBody($@"
                <p>Hello {{{{UserName}}}},</p>
                <p style=""color: #27ae60; font-weight: bold;"">✓ Two-factor authentication has been successfully enabled on your account.</p>
                <p>This was enabled on {{{{EnabledAt}}}}.</p>
                <p>Your account is now more secure! You'll need to enter a verification code from your authenticator app each time you sign in.</p>
                {new HtmlEmailBuilder().CreateDivider()}
                <p style=""font-size: 14px; color: #666666;"">If you didn't make this change, please contact our support team immediately.</p>
            ")
            .WithSimpleFooter("{{AppName}}")
            .Build(),
        IsHtml = true,
        DefaultPriority = EmailPriority.Normal
    };

    /// <summary>
    /// Template for new login detection alerts.
    /// Placeholders: {{UserName}}, {{AppName}}, {{LoginTime}}, {{IpAddress}}, {{Location}}, {{Device}}, {{ReportLink}}
    /// </summary>
    public static IEmailTemplate NewLoginDetected => new EmailTemplate
    {
        Subject = "New sign-in to your account",
        BodyTemplate = new HtmlEmailBuilder()
            .WithTextHeader("New Sign-In Detected")
            .WithBody($@"
                <p>Hello {{{{UserName}}}},</p>
                <p>We detected a new sign-in to your {{{{AppName}}}} account.</p>
                <table role=""presentation"" style=""width: 100%; background-color: #f8f9fa; border-radius: 8px; padding: 20px; margin: 20px 0;"">
                    <tr><td style=""padding: 8px 0;""><strong>Time:</strong></td><td>{{{{LoginTime}}}}</td></tr>
                    <tr><td style=""padding: 8px 0;""><strong>IP Address:</strong></td><td>{{{{IpAddress}}}}</td></tr>
                    <tr><td style=""padding: 8px 0;""><strong>Location:</strong></td><td>{{{{Location}}}}</td></tr>
                    <tr><td style=""padding: 8px 0;""><strong>Device:</strong></td><td>{{{{Device}}}}</td></tr>
                </table>
                <p>If this was you, you can ignore this email.</p>
                <p style=""color: #e74c3c;"">If you don't recognize this activity, please secure your account immediately:</p>
                {new HtmlEmailBuilder().CreateButton("Report Suspicious Activity", "{{ReportLink}}", "#e74c3c")}
            ")
            .WithSimpleFooter("{{AppName}}")
            .Build(),
        IsHtml = true,
        DefaultPriority = EmailPriority.Normal
    };

    /// <summary>
    /// Template for account locked notification.
    /// Placeholders: {{UserName}}, {{AppName}}, {{LockReason}}, {{UnlockLink}}
    /// </summary>
    public static IEmailTemplate AccountLocked => new EmailTemplate
    {
        Subject = "Your account has been locked",
        BodyTemplate = new HtmlEmailBuilder()
            .WithPrimaryColor("#e74c3c")
            .WithTextHeader("Account Locked")
            .WithBody($@"
                <p>Hello {{{{UserName}}}},</p>
                <p style=""color: #e74c3c;"">Your {{{{AppName}}}} account has been temporarily locked for security reasons.</p>
                <p><strong>Reason:</strong> {{{{LockReason}}}}</p>
                <p>To unlock your account, please click the button below:</p>
                {new HtmlEmailBuilder().CreateButton("Unlock Account", "{{UnlockLink}}")}
                <p style=""font-size: 14px; color: #666666;"">If you didn't trigger this lock or need assistance, please contact our support team.</p>
            ")
            .WithSimpleFooter("{{AppName}}")
            .Build(),
        IsHtml = true,
        DefaultPriority = EmailPriority.Urgent
    };

    /// <summary>
    /// Template for password changed notification.
    /// Placeholders: {{UserName}}, {{AppName}}, {{ChangedAt}}, {{ReportLink}}
    /// </summary>
    public static IEmailTemplate PasswordChanged => new EmailTemplate
    {
        Subject = "Your password was changed",
        BodyTemplate = new HtmlEmailBuilder()
            .WithTextHeader("Password Changed")
            .WithBody($@"
                <p>Hello {{{{UserName}}}},</p>
                <p>Your {{{{AppName}}}} account password was successfully changed on {{{{ChangedAt}}}}.</p>
                <p>If you made this change, you can safely ignore this email.</p>
                {new HtmlEmailBuilder().CreateDivider()}
                <p style=""color: #e74c3c;"">If you didn't change your password, your account may have been compromised. Please take action immediately:</p>
                {new HtmlEmailBuilder().CreateButton("Report & Secure Account", "{{ReportLink}}", "#e74c3c")}
            ")
            .WithSimpleFooter("{{AppName}}")
            .Build(),
        IsHtml = true,
        DefaultPriority = EmailPriority.High
    };
}
