# W.Core.Email

A Gmail API wrapper with multi-project templating support for .NET applications.

[![NuGet](https://img.shields.io/nuget/v/W.Core.Email.svg)](https://www.nuget.org/packages/W.Core.Email)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Table of Contents

- [Overview](#overview)
- [Installation](#installation)
- [Quick Start](#quick-start)
- [Architecture](#architecture)
- [Project Structure](#project-structure)
- [Capabilities](#capabilities)
- [Configuration](#configuration)
- [Templating System](#templating-system)
- [HTML Email Builder](#html-email-builder)
- [Pre-Built Auth Templates](#pre-built-auth-templates)
- [Custom Email Options](#custom-email-options)
- [Exception Handling](#exception-handling)
- [Multi-Project Support](#multi-project-support)
- [Pros and Cons](#pros-and-cons)
- [Requirements](#requirements)
- [License](#license)

---

## Overview

W.Core.Email is a .NET library that wraps the Gmail API, providing a high-level abstraction for sending templated emails. It's designed to support multiple consuming projects, each with their own isolated template registries, making it ideal for shared infrastructure in microservices or multi-tenant applications.

### Key Design Goals

- **Simplicity**: Fluent configuration API with sensible defaults
- **Multi-tenancy**: Project-level template isolation via composite keys
- **Type Safety**: Strongly-typed options and models with validation
- **Extensibility**: Interface-based design for testing and customization
- **Compatibility**: Multi-targeting for broad .NET ecosystem support

---

## Installation

```bash
dotnet add package W.Core.Email
```

**Package Dependencies:**
- `Google.Apis.Gmail.v1` - Gmail API client
- `MimeKit` - MIME message construction
- `Microsoft.Extensions.DependencyInjection.Abstractions` - DI support
- `Microsoft.Extensions.Options` - Options pattern support
- `Microsoft.Extensions.Logging.Abstractions` - Logging support

---

## Quick Start

### 1. Register Services

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
});
```

### 2. Inject and Send

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
            _logger.LogError("Email failed: {Error}", result.ErrorMessage);
        }
    }
}
```

---

## Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                        Consumer Application                      │
├─────────────────────────────────────────────────────────────────┤
│  IEmailService                                                   │
│    ├── SendAsync(templateType, to, model, options)              │
│    └── SendRawAsync(message)                                    │
├─────────────────────────────────────────────────────────────────┤
│  EmailService                                                    │
│    ├── Template Lookup (via TemplateRegistry)                   │
│    ├── Template Rendering (via TemplateRenderer)                │
│    └── Message Building (merges options + defaults)             │
├─────────────────────────────────────────────────────────────────┤
│  IGmailClient                                                    │
│    ├── OAuth / Service Account Authentication                   │
│    ├── MIME Message Construction (via MimeMessageBuilder)       │
│    └── Retry with Exponential Backoff                           │
├─────────────────────────────────────────────────────────────────┤
│  Gmail API                                                       │
└─────────────────────────────────────────────────────────────────┘
```

### Data Flow

1. **Consumer** calls `IEmailService.SendAsync()` with template type, recipient, and model
2. **EmailService** looks up the template from `ITemplateRegistry` using composite key (ProjectKey + TemplateType)
3. **TemplateRenderer** replaces `{{placeholders}}` with model values using reflection
4. **EmailService** builds `EmailMessage` by merging template defaults, global options, and per-send options
5. **GmailClient** converts to MIME format and sends via Gmail API with retry logic

---

## Project Structure

```
W.Core.Email/
├── src/W.Core.Email/
│   ├── Configuration/
│   │   ├── EmailOptions.cs              # Global email settings
│   │   ├── GmailAuthOptions.cs          # Gmail authentication config
│   │   ├── EmailOptionsValidator.cs     # Options validation
│   │   ├── GmailOptionsValidator.cs     # Gmail options validation
│   │   ├── ServiceCollectionExtensions.cs # DI registration
│   │   ├── EmailServiceBuilder.cs       # Fluent configuration builder
│   │   └── AuthTemplateExtensions.cs    # W.Auth template registration
│   │
│   ├── Models/
│   │   ├── EmailMessage.cs              # Email message DTO
│   │   ├── EmailAttachment.cs           # Attachment model
│   │   ├── EmailPriority.cs             # Priority enum
│   │   ├── SendResult.cs                # Operation result
│   │   └── SendEmailOptions.cs          # Per-send overrides
│   │
│   ├── Templating/
│   │   ├── IEmailTemplate.cs            # Template interface
│   │   ├── EmailTemplate.cs             # Template implementation
│   │   ├── TemplateKey.cs               # Composite key record
│   │   ├── ITemplateRegistry.cs         # Registry interface
│   │   ├── TemplateRegistry.cs          # Thread-safe registry
│   │   ├── TemplateRenderer.cs          # Placeholder replacement
│   │   ├── HtmlEmailBuilder.cs          # Styled HTML builder
│   │   └── AuthTemplates.cs             # Pre-built auth templates
│   │
│   ├── Services/
│   │   ├── IGmailClient.cs              # Gmail client interface
│   │   ├── GmailClient.cs               # Gmail API implementation
│   │   └── EmailService.cs              # High-level service
│   │
│   ├── Exceptions/
│   │   ├── EmailException.cs            # Base exception
│   │   ├── EmailErrorCode.cs            # Error code enum
│   │   ├── TemplateNotFoundException.cs # Missing template
│   │   ├── AuthenticationException.cs   # Auth failures
│   │   ├── RateLimitException.cs        # Gmail rate limiting
│   │   └── SendFailedException.cs       # Send failures
│   │
│   ├── Internal/
│   │   ├── MimeMessageBuilder.cs        # MIME construction
│   │   └── Polyfills.cs                 # netstandard2.0 compatibility
│   │
│   └── IEmailService.cs                 # Main service interface
│
├── tests/W.Core.Email.Tests/            # 94 unit tests
└── samples/SampleEmailApp/              # Usage examples
```

---

## Capabilities

### Email Sending
- **Templated emails**: Send using registered templates with model binding
- **Raw emails**: Send arbitrary `EmailMessage` objects directly
- **Multiple recipients**: Send to multiple To, CC, and BCC addresses
- **Attachments**: Attach files with MIME type detection
- **Priority levels**: Normal, High, Urgent priorities with header support

### Authentication
- **OAuth 2.0**: Interactive browser-based authentication with token persistence
- **Service Account**: Server-to-server authentication with domain delegation

### Template System
- **Placeholder syntax**: `{{PropertyName}}` with case-insensitive matching
- **Format specifiers**: `{{Date:yyyy-MM-dd}}` for IFormattable types
- **Model binding**: Supports POCOs, anonymous types, and dictionaries
- **Multi-project isolation**: Templates scoped by ProjectKey

### HTML Emails
- **Responsive design**: Mobile-friendly table-based layout
- **Customizable styling**: Colors, fonts, preheader text
- **Components**: Buttons, headings, paragraphs, dividers
- **Pre-built templates**: 6 authentication flow templates included

### Reliability
- **Retry logic**: Exponential backoff for transient failures (503, 504)
- **Rate limit handling**: Detects 429 responses and throws `RateLimitException`
- **Validation**: Options validated at startup via `IValidateOptions<T>`

---

## Configuration

### EmailOptions

```csharp
services.AddEmail(options =>
{
    options.ProjectKey = "MyProject";           // Required: Unique project identifier
    options.DefaultSenderName = "My App";       // Optional: Default "From" name
    options.DefaultSenderEmail = "no-reply@x.com"; // Optional: Default "From" address
    options.DefaultReplyTo = "support@x.com";   // Optional: Default Reply-To
    options.TrackOpensByDefault = false;        // Optional: Open tracking
    options.TrackClicksByDefault = false;       // Optional: Click tracking
});
```

### GmailAuthOptions

```csharp
.AddGmailProvider(gmail =>
{
    gmail.CredentialsPath = "credentials.json"; // Required: Path to credentials
    gmail.SenderEmail = "sender@domain.com";    // Required: Authorized sender
    gmail.ApplicationName = "My Application";   // Optional: App name for API
    gmail.AuthMethod = GmailAuthMethod.OAuth;   // Optional: OAuth or ServiceAccount
    gmail.TokenStorePath = "./tokens";          // Optional: Token storage location
})
```

---

## Templating System

### Template Registration

```csharp
// Simple text template
.AddTextTemplate("SimpleAlert",
    "Alert: {{Title}}",
    "Alert message: {{Message}}")

// HTML template
.AddHtmlTemplate("RichAlert",
    "Alert: {{Title}}",
    "<h1>{{Title}}</h1><p>{{Message}}</p>")

// Full control
.AddTemplate("Custom", new EmailTemplate
{
    Subject = "{{Subject}}",
    BodyTemplate = "{{Body}}",
    IsHtml = true,
    DefaultPriority = EmailPriority.High
})
```

### Placeholder Syntax

```csharp
// Basic replacement
"Hello {{UserName}}"

// With format specifier (for dates, numbers, etc.)
"Created on {{CreatedAt:MMMM dd, yyyy}}"
"Total: {{Amount:C2}}"

// Case-insensitive matching
"{{username}}" matches property "UserName"
```

### Model Binding

```csharp
// Anonymous type
await _emailService.SendAsync("Welcome", email, new
{
    UserName = "John",
    AppName = "MyApp"
});

// POCO
await _emailService.SendAsync("Welcome", email, new WelcomeModel
{
    UserName = "John",
    AppName = "MyApp"
});

// Dictionary
await _emailService.SendAsync("Welcome", email, new Dictionary<string, object?>
{
    ["UserName"] = "John",
    ["AppName"] = "MyApp"
});
```

---

## HTML Email Builder

Build responsive, styled HTML emails programmatically:

```csharp
var html = new HtmlEmailBuilder()
    .WithPrimaryColor("#3498db")
    .WithBackgroundColor("#f4f4f4")
    .WithPreheader("Preview text shown in email clients")
    .WithLogoHeader("https://example.com/logo.png", "Company Logo", 150)
    .WithBody($@"
        {builder.CreateHeading("Welcome!", 1)}
        {builder.CreateParagraph("Thanks for signing up.")}
        {builder.CreateButton("Get Started", "https://example.com/start")}
        {builder.CreateDivider()}
        {builder.CreateParagraph("Questions? Contact support.")}
    ")
    .WithSimpleFooter("My Company", "https://example.com/unsubscribe")
    .Build();
```

### Available Methods

| Method | Description |
|--------|-------------|
| `WithPreheader(text)` | Hidden preview text for email clients |
| `WithHeader(html)` | Custom header HTML |
| `WithLogoHeader(url, alt, width)` | Centered logo image |
| `WithTextHeader(text)` | Styled text header |
| `WithBody(html)` | Main content area |
| `WithFooter(html)` | Custom footer HTML |
| `WithSimpleFooter(company, unsubscribe?)` | Standard footer with copyright |
| `WithPrimaryColor(hex)` | Brand/accent color |
| `WithBackgroundColor(hex)` | Email background |
| `WithTextColor(hex)` | Body text color |
| `WithFontFamily(family)` | Font stack |
| `CreateButton(text, url, color?)` | CTA button HTML |
| `CreateHeading(text, level)` | h1-h4 heading |
| `CreateParagraph(text)` | Styled paragraph |
| `CreateDivider()` | Horizontal rule |

---

## Pre-Built Auth Templates

Register all authentication templates with one call:

```csharp
services.AddEmail(o => o.ProjectKey = "MyAuth")
    .AddGmailProvider(...)
    .AddAuthTemplates();  // Registers all 6 templates
```

### Available Templates

| Template | Placeholders | Priority |
|----------|--------------|----------|
| `EmailConfirmation` | `UserName`, `ConfirmationLink`, `AppName`, `ExpiryHours` | Normal |
| `PasswordReset` | `UserName`, `ResetLink`, `AppName`, `ExpiryMinutes` | High |
| `TwoFactorEnabled` | `UserName`, `AppName`, `EnabledAt` | Normal |
| `NewLoginDetected` | `UserName`, `AppName`, `LoginTime`, `IpAddress`, `Location`, `Device`, `ReportLink` | Normal |
| `AccountLocked` | `UserName`, `AppName`, `LockReason`, `UnlockLink` | Urgent |
| `PasswordChanged` | `UserName`, `AppName`, `ChangedAt`, `ReportLink` | High |

### Usage Example

```csharp
await _emailService.SendAsync("PasswordReset", user.Email, new
{
    UserName = user.DisplayName,
    ResetLink = resetUrl,
    AppName = "My Application",
    ExpiryMinutes = 15
});
```

---

## Custom Email Options

Override defaults on a per-send basis:

```csharp
await _emailService.SendAsync("Invoice", customer.Email, model, new SendEmailOptions
{
    Priority = EmailPriority.High,
    Cc = new[] { "billing@company.com" },
    Bcc = new[] { "archive@company.com" },
    ReplyTo = "support@company.com",
    FromName = "Billing Department",
    Attachments = new[]
    {
        new EmailAttachment
        {
            FileName = "invoice.pdf",
            Content = pdfBytes,
            ContentType = "application/pdf"
        }
    },
    CustomHeaders = new Dictionary<string, string>
    {
        ["X-Invoice-Id"] = invoiceId
    },
    TrackOpens = true,
    TrackClicks = true
});
```

### Static Factory Methods

```csharp
// Quick priority setting
SendEmailOptions.HighPriority()
SendEmailOptions.Urgent()

// Quick attachments
SendEmailOptions.WithAttachments(attachment1, attachment2)

// Quick CC
SendEmailOptions.WithCc("manager@company.com", "team@company.com")
```

---

## Exception Handling

All exceptions inherit from `EmailException` with structured error codes:

```csharp
try
{
    await _emailService.SendAsync("Welcome", email, model);
}
catch (TemplateNotFoundException ex)
{
    // Template not registered for this project
    _logger.LogError("Template {Type} not found for {Project}",
        ex.TemplateType, ex.ProjectKey);
}
catch (AuthenticationException ex)
{
    // Gmail authentication failed
    _logger.LogError("Auth failed ({Method}): {Message}",
        ex.AuthMethod, ex.Message);
}
catch (RateLimitException ex)
{
    // Gmail rate limit hit
    _logger.LogWarning("Rate limited, retry after {Delay}", ex.RetryAfter);
    await Task.Delay(ex.RetryAfter);
}
catch (SendFailedException ex)
{
    // Gmail API error
    _logger.LogError("Send failed: {Message}", ex.Message);
}
catch (EmailException ex)
{
    // Any other email error
    _logger.LogError("Email error ({Code}): {Message}",
        ex.ErrorCode, ex.Message);
}
```

### Error Codes

| Code | Exception | Description |
|------|-----------|-------------|
| `TemplateNotFound` | `TemplateNotFoundException` | Template not registered |
| `AuthenticationFailed` | `AuthenticationException` | OAuth/SA auth failed |
| `RateLimitExceeded` | `RateLimitException` | Gmail quota exceeded |
| `SendFailed` | `SendFailedException` | Gmail API error |
| `ValidationFailed` | `EmailException` | Invalid message data |
| `Unknown` | `EmailException` | Unexpected error |

---

## Multi-Project Support

Each consuming project registers templates under its own namespace:

```csharp
// In Project A (e.g., AuthService)
services.AddEmail(o => o.ProjectKey = "AuthService")
    .AddGmailProvider(...)
    .AddTemplate("Notification", authNotificationTemplate);

// In Project B (e.g., OrderService)
services.AddEmail(o => o.ProjectKey = "OrderService")
    .AddGmailProvider(...)
    .AddTemplate("Notification", orderNotificationTemplate);  // Different template, same name!
```

Templates are stored with composite keys: `(ProjectKey, TemplateType)`. This allows:
- Multiple projects to use identical template names without collision
- Shared template registry singleton across all projects
- Project-specific template customization

---

## Pros and Cons

### Pros

| Aspect | Details |
|--------|---------|
| **Multi-targeting** | Supports both `net10.0` and `netstandard2.0`, enabling use in legacy and modern projects |
| **Thread-safe** | `ConcurrentDictionary` ensures safe concurrent template access |
| **Fluent API** | Intuitive builder pattern for configuration |
| **DI-first** | Built for `Microsoft.Extensions.DependencyInjection` with proper lifetime management |
| **Validation** | Options validated at startup, failing fast on misconfiguration |
| **Retry logic** | Automatic exponential backoff for transient Gmail failures |
| **Structured exceptions** | Custom exception hierarchy with error codes for precise handling |
| **Pre-built templates** | 6 production-ready auth templates save development time |
| **HTML builder** | Programmatic responsive email generation without HTML expertise |
| **Interface-based** | `IEmailService`, `IGmailClient`, `ITemplateRegistry` enable testing and extension |

### Cons

| Aspect | Details |
|--------|---------|
| **Gmail-only** | Currently only supports Gmail API; no SMTP, SendGrid, SES, etc. |
| **Simple templating** | No conditionals, loops, or includes - just placeholder replacement |
| **No template caching** | Templates rendered on each send; no compiled template optimization |
| **Reflection-based** | Model binding uses reflection which has performance overhead |
| **No batch sending** | Each email is a separate API call; no bulk send optimization |
| **OAuth complexity** | OAuth flow requires browser interaction; challenging for headless environments |
| **No tracking implementation** | `TrackOpens`/`TrackClicks` options exist but require external tracking service |
| **Singleton GmailClient** | Single credential set per application; can't use multiple Gmail accounts |

### Trade-offs Made

1. **Simplicity over Power**: The `{{placeholder}}` syntax was chosen over a full templating engine (Razor, Liquid) to minimize dependencies and complexity. For complex templates, generate HTML externally.

2. **Reflection over Source Generators**: Model binding uses runtime reflection for compatibility with `netstandard2.0`. A source generator approach would be faster but limit compatibility.

3. **Singleton Registry**: The template registry is a singleton for simplicity. This means templates registered after startup won't be available to already-constructed services.

4. **Synchronous Template Registration**: Templates are registered during DI configuration, not lazily loaded. This ensures fail-fast but requires all templates to be known at startup.

---

## Requirements

- **.NET 10.0+** or **.NET Standard 2.0+** (.NET Framework 4.6.1+, .NET Core 2.0+)
- **Google Cloud Project** with Gmail API enabled
- **OAuth credentials** (for user context) or **Service Account** (for server-to-server)

### Google Cloud Setup

1. Go to [Google Cloud Console](https://console.cloud.google.com/)
2. Create or select a project
3. Enable the **Gmail API**
4. Create credentials:
   - **OAuth 2.0**: For applications acting on behalf of users
   - **Service Account**: For server applications with domain-wide delegation
5. Download the credentials JSON file

---

## License

MIT License - see [LICENSE](LICENSE) for details.

---

## Contributing

Contributions are welcome! Please:
1. Fork the repository
2. Create a feature branch
3. Write tests for new functionality
4. Ensure all tests pass (`dotnet test`)
5. Submit a pull request

---

*Built with the Gmail API, MimeKit, and Microsoft.Extensions.*
