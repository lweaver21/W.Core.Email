using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using W.Core.Email;
using W.Core.Email.Configuration;
using W.Core.Email.Models;
using W.Core.Email.Templating;

Console.WriteLine("W.Core.Email - Sample Application");
Console.WriteLine("==================================\n");

// Build service provider
var services = new ServiceCollection();

// Add logging
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});

// Configure email service
services.AddEmail(options =>
{
    options.ProjectKey = "SampleApp";
    options.DefaultSenderName = "Sample App";
})
.AddGmailProvider(gmail =>
{
    // In production, use configuration or environment variables
    gmail.CredentialsPath = "credentials.json";
    gmail.SenderEmail = "your-email@gmail.com";
    gmail.AuthMethod = GmailAuthMethod.OAuth;
})
// Add custom templates
.AddHtmlTemplate("Welcome",
    "Welcome to {{AppName}}!",
    new HtmlEmailBuilder()
        .WithTextHeader("Welcome!")
        .WithBody(@"
            <p>Hello {{UserName}},</p>
            <p>Welcome to {{AppName}}! We're excited to have you.</p>
        ")
        .WithSimpleFooter("{{AppName}}")
        .Build())
// Add auth templates
.AddAuthTemplates();

var provider = services.BuildServiceProvider();

// Initialize templates (required for template registration)
_ = provider.GetService<TemplateRegistryInitializer>();

// Get the email service
var emailService = provider.GetRequiredService<IEmailService>();
var logger = provider.GetRequiredService<ILogger<Program>>();

// Demo: Show available templates
Console.WriteLine("Registered Templates:");
var registry = provider.GetRequiredService<ITemplateRegistry>();
foreach (var templateType in registry.GetTemplateTypes("SampleApp"))
{
    Console.WriteLine($"  - {templateType}");
}
Console.WriteLine();

// Demo: Render a template (without sending)
Console.WriteLine("Template Rendering Demo:");
Console.WriteLine("------------------------");
var renderer = provider.GetRequiredService<TemplateRenderer>();
var welcomeTemplate = registry.Get("SampleApp", "Welcome");
var (subject, body) = renderer.RenderTemplate(welcomeTemplate, new
{
    AppName = "Sample App",
    UserName = "John Doe"
});
Console.WriteLine($"Subject: {subject}");
Console.WriteLine($"Body preview: {body.Substring(0, Math.Min(200, body.Length))}...\n");

// Demo: Send email (uncomment to actually send)
Console.WriteLine("Email Sending Demo:");
Console.WriteLine("-------------------");
Console.WriteLine("To actually send emails, you need to:");
Console.WriteLine("1. Copy 'credentials.json.template' to 'credentials.json'");
Console.WriteLine("2. Add your Google OAuth credentials");
Console.WriteLine("3. Update the sender email in Program.cs");
Console.WriteLine("4. Uncomment the send code below\n");

/*
// Uncomment to send a test email
var result = await emailService.SendAsync(
    "EmailConfirmation",
    "recipient@example.com",
    new
    {
        UserName = "John Doe",
        AppName = "Sample App",
        ConfirmationLink = "https://example.com/confirm?token=abc123",
        ExpiryHours = 24
    });

if (result.Success)
{
    Console.WriteLine($"Email sent successfully! Message ID: {result.MessageId}");
}
else
{
    Console.WriteLine($"Failed to send email: {result.ErrorMessage}");
}
*/

// Demo: Show raw email building
Console.WriteLine("Raw Email Demo:");
Console.WriteLine("---------------");
var rawMessage = EmailMessage.Create(
    "recipient@example.com",
    "Test Subject",
    "<h1>Hello!</h1><p>This is a raw HTML email.</p>",
    isHtml: true);
rawMessage.Priority = EmailPriority.High;
rawMessage.Cc.Add("cc@example.com");

Console.WriteLine($"To: {string.Join(", ", rawMessage.To)}");
Console.WriteLine($"CC: {string.Join(", ", rawMessage.Cc)}");
Console.WriteLine($"Subject: {rawMessage.Subject}");
Console.WriteLine($"Priority: {rawMessage.Priority}");
Console.WriteLine($"Is HTML: {rawMessage.IsHtml}");
Console.WriteLine();

Console.WriteLine("Sample application complete!");
