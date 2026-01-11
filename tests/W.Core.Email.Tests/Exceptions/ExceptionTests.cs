using FluentAssertions;
using W.Core.Email.Exceptions;
using Xunit;

namespace W.Core.Email.Tests.Exceptions;

public class ExceptionTests
{
    [Fact]
    public void EmailException_SetsMessageAndCode()
    {
        var ex = new EmailException("Test error", EmailErrorCode.SendFailed);

        ex.Message.Should().Be("Test error");
        ex.ErrorCode.Should().Be(EmailErrorCode.SendFailed);
    }

    [Fact]
    public void EmailException_WithInnerException_PreservesIt()
    {
        var inner = new InvalidOperationException("Inner");
        var ex = new EmailException("Outer", inner, EmailErrorCode.Unknown);

        ex.InnerException.Should().BeSameAs(inner);
    }

    [Fact]
    public void TemplateNotFoundException_SetsProperties()
    {
        var ex = new TemplateNotFoundException("ProjectA", "WelcomeEmail");

        ex.ProjectKey.Should().Be("ProjectA");
        ex.TemplateType.Should().Be("WelcomeEmail");
        ex.ErrorCode.Should().Be(EmailErrorCode.TemplateNotFound);
        ex.Message.Should().Contain("WelcomeEmail").And.Contain("ProjectA");
    }

    [Fact]
    public void AuthenticationException_SetsAuthMethod()
    {
        var ex = new AuthenticationException("Auth failed", "OAuth");

        ex.AuthMethod.Should().Be("OAuth");
        ex.ErrorCode.Should().Be(EmailErrorCode.AuthenticationFailed);
    }

    [Fact]
    public void AuthenticationException_InvalidCredentials_Factory()
    {
        var ex = AuthenticationException.InvalidCredentials("Bad key");

        ex.Message.Should().Contain("Invalid credentials").And.Contain("Bad key");
    }

    [Fact]
    public void AuthenticationException_CredentialsFileNotFound_Factory()
    {
        var ex = AuthenticationException.CredentialsFileNotFound("/path/to/file");

        ex.Message.Should().Contain("not found").And.Contain("/path/to/file");
    }

    [Fact]
    public void AuthenticationException_TokenExpired_Factory()
    {
        var ex = AuthenticationException.TokenExpired();

        ex.Message.Should().Contain("expired");
        ex.AuthMethod.Should().Be("OAuth");
    }

    [Fact]
    public void RateLimitException_SetsRetryAfter()
    {
        var retryAfter = TimeSpan.FromSeconds(30);
        var ex = new RateLimitException("Rate limited", retryAfter);

        ex.RetryAfter.Should().Be(retryAfter);
        ex.ErrorCode.Should().Be(EmailErrorCode.RateLimitExceeded);
    }

    [Fact]
    public void RateLimitException_SetsQuotaInfo()
    {
        var ex = new RateLimitException("Quota exceeded", 100, 101, TimeSpan.FromHours(1));

        ex.QuotaLimit.Should().Be(100);
        ex.CurrentUsage.Should().Be(101);
    }

    [Fact]
    public void RateLimitException_DailyQuotaExceeded_Factory()
    {
        var ex = RateLimitException.DailyQuotaExceeded(500);

        ex.Message.Should().Contain("500");
        ex.ErrorCode.Should().Be(EmailErrorCode.QuotaExceeded);
    }

    [Fact]
    public void RateLimitException_PerSecondLimitExceeded_Factory()
    {
        var ex = RateLimitException.PerSecondLimitExceeded(TimeSpan.FromSeconds(5));

        ex.RetryAfter.Should().Be(TimeSpan.FromSeconds(5));
    }

    [Fact]
    public void SendFailedException_SetsErrorCode()
    {
        var ex = new SendFailedException("Send failed", EmailErrorCode.InvalidRecipient);

        ex.ErrorCode.Should().Be(EmailErrorCode.InvalidRecipient);
    }

    [Fact]
    public void SendFailedException_SetsFailedRecipients()
    {
        var recipients = new[] { "bad1@example.com", "bad2@example.com" };
        var ex = new SendFailedException("Failed", recipients);

        ex.FailedRecipients.Should().BeEquivalentTo(recipients);
    }

    [Fact]
    public void SendFailedException_InvalidRecipient_Factory()
    {
        var ex = SendFailedException.InvalidRecipient("invalid@");

        ex.Message.Should().Contain("invalid@");
        ex.ErrorCode.Should().Be(EmailErrorCode.InvalidRecipient);
    }

    [Fact]
    public void SendFailedException_InvalidSender_Factory()
    {
        var ex = SendFailedException.InvalidSender("notauthorized@example.com");

        ex.Message.Should().Contain("notauthorized@example.com");
        ex.ErrorCode.Should().Be(EmailErrorCode.InvalidSender);
    }

    [Fact]
    public void SendFailedException_AttachmentTooLarge_Factory()
    {
        var ex = SendFailedException.AttachmentTooLarge("big.zip", 30_000_000, 25_000_000);

        ex.Message.Should().Contain("big.zip");
        ex.ErrorCode.Should().Be(EmailErrorCode.AttachmentTooLarge);
    }

    [Fact]
    public void SendFailedException_MessageTooLarge_Factory()
    {
        var ex = SendFailedException.MessageTooLarge(50_000_000, 35_000_000);

        ex.ErrorCode.Should().Be(EmailErrorCode.MessageTooLarge);
    }
}
