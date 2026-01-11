using FluentAssertions;
using W.Core.Email.Configuration;
using Xunit;

namespace W.Core.Email.Tests.Configuration;

public class GmailOptionsValidatorTests
{
    private readonly GmailOptionsValidator _validator = new();

    [Fact]
    public void Validate_ValidOptions_ReturnsSuccess()
    {
        var options = new GmailAuthOptions
        {
            CredentialsPath = "/path/to/credentials.json",
            SenderEmail = "sender@example.com",
            ApplicationName = "MyApp"
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyCredentialsPath_ReturnsFail()
    {
        var options = new GmailAuthOptions
        {
            CredentialsPath = "",
            SenderEmail = "sender@example.com"
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain("CredentialsPath");
    }

    [Fact]
    public void Validate_EmptySenderEmail_ReturnsFail()
    {
        var options = new GmailAuthOptions
        {
            CredentialsPath = "/path/to/credentials.json",
            SenderEmail = ""
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain("SenderEmail");
    }

    [Fact]
    public void Validate_InvalidSenderEmail_ReturnsFail()
    {
        var options = new GmailAuthOptions
        {
            CredentialsPath = "/path/to/credentials.json",
            SenderEmail = "not-an-email"
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain("valid email");
    }

    [Fact]
    public void Validate_EmptyApplicationName_ReturnsFail()
    {
        var options = new GmailAuthOptions
        {
            CredentialsPath = "/path/to/credentials.json",
            SenderEmail = "sender@example.com",
            ApplicationName = ""
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain("ApplicationName");
    }

    [Fact]
    public void Validate_DefaultAuthMethod_IsOAuth()
    {
        var options = new GmailAuthOptions();

        options.AuthMethod.Should().Be(GmailAuthMethod.OAuth);
    }

    [Fact]
    public void Validate_DefaultApplicationName_IsSet()
    {
        var options = new GmailAuthOptions();

        options.ApplicationName.Should().Be("W.Core.Email");
    }
}
