# W.Core.Email

A Gmail API wrapper with multi-project templating support for .NET applications.

## Features

- **Gmail API Integration** - Send emails via Gmail API with OAuth or Service Account authentication
- **Multi-Project Templating** - Each consuming project registers its own templates by type
- **Template Rendering** - Simple `{{placeholder}}` syntax with model binding
- **Custom Email Options** - Per-send customization (priority, attachments, CC/BCC, tracking)
- **Dependency Injection** - First-class support for Microsoft.Extensions.DependencyInjection

## Installation

```bash
dotnet add package W.Core.Email
```

## Quick Start

### 1. Register the Email Service

```csharp
builder.Services.AddEmail(options =>
{
    options.ProjectKey = "MyProject";
    options.DefaultSenderName = "My Application";
})
.AddGmailProvider(gmail =>
{
    gmail.CredentialsPath = "credentials.json";
    gmail.SenderEmail = "noreply@mycompany.com";
})
.AddTemplate("Welcome", new EmailTemplate
{
    Subject = "Welcome to {{AppName}}!",
    BodyTemplate = "<h1>Hello {{UserName}}</h1><p>Welcome aboard!</p>",
    IsHtml = true
})
.AddTemplate("PasswordReset", new EmailTemplate
{
    Subject = "Password Reset Request",
    BodyTemplate = "Click here to reset: {{ResetUrl}}",
    IsHtml = false,
    DefaultPriority = EmailPriority.High
});
```

### 2. Send Emails

```csharp
public class AccountService
{
    private readonly IEmailService _emailService;

    public AccountService(IEmailService emailService)
    {
        _emailService = emailService;
    }

    public async Task SendWelcomeEmail(User user)
    {
        var result = await _emailService.SendAsync(
            "Welcome",
            user.Email,
            new { AppName = "MyApp", UserName = user.Name }
        );

        if (!result.Success)
        {
            // Handle error
        }
    }
}
```

## Multi-Project Support

Different projects can register their own templates under unique project keys:

```csharp
// Project A
services.AddEmail(o => o.ProjectKey = "ProjectA")
    .AddTemplate("Notification", projectATemplate);

// Project B
services.AddEmail(o => o.ProjectKey = "ProjectB")
    .AddTemplate("Notification", projectBTemplate); // Different template, same name
```

## Custom Email Options

Override defaults on a per-send basis:

```csharp
await _emailService.SendAsync("Welcome", "user@example.com", model, new SendEmailOptions
{
    Priority = EmailPriority.High,
    Cc = new[] { "admin@example.com" },
    Attachments = new[] { attachment },
    TrackOpens = true
});
```

## Requirements

- .NET 9.0+ or .NET Standard 2.0+
- Google Cloud Project with Gmail API enabled
- OAuth credentials or Service Account with domain delegation

## License

MIT License
