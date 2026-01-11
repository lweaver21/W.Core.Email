# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-01-11

### Added
- Initial release of W.Core.Email
- Gmail API integration with OAuth and Service Account authentication
- Multi-project template registry for isolated template management
- Template rendering with `{{placeholder}}` syntax and format support
- HtmlEmailBuilder for creating styled responsive HTML emails
- Pre-built authentication templates:
  - EmailConfirmation
  - PasswordReset
  - TwoFactorEnabled
  - NewLoginDetected
  - AccountLocked
  - PasswordChanged
- Custom email options (priority, CC/BCC, attachments, custom headers)
- Comprehensive exception handling with error codes
- Retry logic with exponential backoff
- Support for both net10.0 and netstandard2.0
- Sample application with usage examples

### Dependencies
- Google.Apis.Gmail.v1 1.69.0.3742
- MimeKit 4.10.0
- Microsoft.Extensions.DependencyInjection.Abstractions 9.0.1
- Microsoft.Extensions.Options 9.0.1
- Microsoft.Extensions.Logging.Abstractions 9.0.1
