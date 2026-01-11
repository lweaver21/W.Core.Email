# Sample Email Application

This sample demonstrates how to use the W.Core.Email library.

## Setup

1. **Create Google Cloud Project**
   - Go to [Google Cloud Console](https://console.cloud.google.com/)
   - Create a new project or select an existing one
   - Enable the Gmail API

2. **Create OAuth Credentials**
   - Go to APIs & Services > Credentials
   - Click "Create Credentials" > "OAuth client ID"
   - Select "Desktop app" as the application type
   - Download the credentials JSON file

3. **Configure the Application**
   - Copy `credentials.json.template` to `credentials.json`
   - Replace the values with your actual OAuth credentials
   - Update `appsettings.json` with your sender email

4. **Run the Application**
   ```bash
   dotnet run
   ```

## Usage Examples

### Template-based Sending

```csharp
var result = await emailService.SendAsync(
    "EmailConfirmation",
    "user@example.com",
    new
    {
        UserName = "John",
        AppName = "My App",
        ConfirmationLink = "https://example.com/confirm",
        ExpiryHours = 24
    });
```

### Raw Email Sending

```csharp
var message = EmailMessage.Create(
    "recipient@example.com",
    "Hello World",
    "<h1>Hello!</h1><p>This is an HTML email.</p>",
    isHtml: true);

var result = await emailService.SendRawAsync(message);
```

### Custom Options

```csharp
var result = await emailService.SendAsync(
    "Welcome",
    "user@example.com",
    new { UserName = "John" },
    new SendEmailOptions
    {
        Priority = EmailPriority.High,
        Cc = new[] { "manager@example.com" },
        TrackOpens = true
    });
```

## Available Templates

After calling `AddAuthTemplates()`, these templates are available:

| Template | Placeholders |
|----------|--------------|
| EmailConfirmation | UserName, ConfirmationLink, AppName, ExpiryHours |
| PasswordReset | UserName, ResetLink, AppName, ExpiryMinutes |
| TwoFactorEnabled | UserName, AppName, EnabledAt |
| NewLoginDetected | UserName, AppName, LoginTime, IpAddress, Location, Device, ReportLink |
| AccountLocked | UserName, AppName, LockReason, UnlockLink |
| PasswordChanged | UserName, AppName, ChangedAt, ReportLink |
