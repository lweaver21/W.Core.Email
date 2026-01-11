using FluentAssertions;
using W.Core.Email.Configuration;
using Xunit;

namespace W.Core.Email.Tests.Configuration;

public class EmailOptionsValidatorTests
{
    private readonly EmailOptionsValidator _validator = new();

    [Fact]
    public void Validate_ValidOptions_ReturnsSuccess()
    {
        var options = new EmailOptions
        {
            ProjectKey = "MyProject",
            DefaultSenderName = "Test Sender",
            DefaultSenderEmail = "test@example.com"
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_EmptyProjectKey_ReturnsFail()
    {
        var options = new EmailOptions
        {
            ProjectKey = ""
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain("ProjectKey");
    }

    [Fact]
    public void Validate_WhitespaceProjectKey_ReturnsFail()
    {
        var options = new EmailOptions
        {
            ProjectKey = "   "
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public void Validate_ProjectKeyTooLong_ReturnsFail()
    {
        var options = new EmailOptions
        {
            ProjectKey = new string('a', 51)
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain("50 characters");
    }

    [Theory]
    [InlineData("My Project")]
    [InlineData("Project@Name")]
    [InlineData("Project.Name")]
    public void Validate_InvalidProjectKeyCharacters_ReturnsFail(string projectKey)
    {
        var options = new EmailOptions
        {
            ProjectKey = projectKey
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain("letters, numbers, hyphens, and underscores");
    }

    [Theory]
    [InlineData("MyProject")]
    [InlineData("my-project")]
    [InlineData("my_project")]
    [InlineData("Project123")]
    [InlineData("W-Auth")]
    public void Validate_ValidProjectKeyFormats_ReturnsSuccess(string projectKey)
    {
        var options = new EmailOptions
        {
            ProjectKey = projectKey
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validate_InvalidDefaultSenderEmail_ReturnsFail()
    {
        var options = new EmailOptions
        {
            ProjectKey = "MyProject",
            DefaultSenderEmail = "not-an-email"
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain("DefaultSenderEmail");
    }

    [Fact]
    public void Validate_InvalidDefaultReplyTo_ReturnsFail()
    {
        var options = new EmailOptions
        {
            ProjectKey = "MyProject",
            DefaultReplyTo = "invalid-email"
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeFalse();
        result.FailureMessage.Should().Contain("DefaultReplyTo");
    }

    [Fact]
    public void Validate_NullOptionalFields_ReturnsSuccess()
    {
        var options = new EmailOptions
        {
            ProjectKey = "MyProject",
            DefaultSenderName = null,
            DefaultSenderEmail = null,
            DefaultReplyTo = null
        };

        var result = _validator.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }
}
