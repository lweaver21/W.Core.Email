using FluentAssertions;
using MimeKit;
using W.Core.Email.Internal;
using W.Core.Email.Models;
using Xunit;

namespace W.Core.Email.Tests.Services;

public class MimeMessageBuilderTests
{
    [Fact]
    public void Build_SimpleEmail_SetsBasicProperties()
    {
        var email = new EmailMessage
        {
            To = { "recipient@example.com" },
            Subject = "Test Subject",
            Body = "Test Body"
        };

        var message = MimeMessageBuilder.Build(email, "sender@example.com");

        message.Subject.Should().Be("Test Subject");
        message.From.Mailboxes.Should().ContainSingle()
            .Which.Address.Should().Be("sender@example.com");
        message.To.Mailboxes.Should().ContainSingle()
            .Which.Address.Should().Be("recipient@example.com");
    }

    [Fact]
    public void Build_WithFromName_SetsDisplayName()
    {
        var email = new EmailMessage
        {
            To = { "recipient@example.com" },
            Subject = "Test",
            Body = "Body",
            FromName = "Custom Sender"
        };

        var message = MimeMessageBuilder.Build(email, "sender@example.com", "Default Name");

        message.From.Mailboxes.Single().Name.Should().Be("Custom Sender");
    }

    [Fact]
    public void Build_WithDefaultFromName_UsesDefault()
    {
        var email = new EmailMessage
        {
            To = { "recipient@example.com" },
            Subject = "Test",
            Body = "Body"
        };

        var message = MimeMessageBuilder.Build(email, "sender@example.com", "Default Name");

        message.From.Mailboxes.Single().Name.Should().Be("Default Name");
    }

    [Fact]
    public void Build_WithCcAndBcc_SetsRecipients()
    {
        var email = new EmailMessage
        {
            To = { "to@example.com" },
            Cc = { "cc@example.com" },
            Bcc = { "bcc@example.com" },
            Subject = "Test",
            Body = "Body"
        };

        var message = MimeMessageBuilder.Build(email, "sender@example.com");

        message.Cc.Mailboxes.Should().ContainSingle()
            .Which.Address.Should().Be("cc@example.com");
        message.Bcc.Mailboxes.Should().ContainSingle()
            .Which.Address.Should().Be("bcc@example.com");
    }

    [Fact]
    public void Build_WithReplyTo_SetsReplyTo()
    {
        var email = new EmailMessage
        {
            To = { "to@example.com" },
            ReplyTo = "reply@example.com",
            Subject = "Test",
            Body = "Body"
        };

        var message = MimeMessageBuilder.Build(email, "sender@example.com");

        message.ReplyTo.Mailboxes.Should().ContainSingle()
            .Which.Address.Should().Be("reply@example.com");
    }

    [Fact]
    public void Build_HtmlBody_SetsHtmlContentType()
    {
        var email = new EmailMessage
        {
            To = { "to@example.com" },
            Subject = "Test",
            Body = "<h1>Hello</h1>",
            IsHtml = true
        };

        var message = MimeMessageBuilder.Build(email, "sender@example.com");

        message.Body.Should().BeOfType<TextPart>();
        ((TextPart)message.Body).Text.Should().Be("<h1>Hello</h1>");
    }

    [Fact]
    public void Build_PlainTextBody_SetsPlainContentType()
    {
        var email = new EmailMessage
        {
            To = { "to@example.com" },
            Subject = "Test",
            Body = "Plain text",
            IsHtml = false
        };

        var message = MimeMessageBuilder.Build(email, "sender@example.com");

        var textPart = message.Body as TextPart;
        textPart.Should().NotBeNull();
        textPart!.Text.Should().Be("Plain text");
    }

    [Fact]
    public void Build_HighPriority_SetsPriorityHeader()
    {
        var email = new EmailMessage
        {
            To = { "to@example.com" },
            Subject = "Test",
            Body = "Body",
            Priority = EmailPriority.High
        };

        var message = MimeMessageBuilder.Build(email, "sender@example.com");

        message.Priority.Should().Be(MessagePriority.Urgent);
    }

    [Fact]
    public void Build_UrgentPriority_SetsXPriorityHeader()
    {
        var email = new EmailMessage
        {
            To = { "to@example.com" },
            Subject = "Test",
            Body = "Body",
            Priority = EmailPriority.Urgent
        };

        var message = MimeMessageBuilder.Build(email, "sender@example.com");

        message.Headers["X-Priority"].Should().Be("1");
    }

    [Fact]
    public void Build_WithCustomHeaders_AddsHeaders()
    {
        var email = new EmailMessage
        {
            To = { "to@example.com" },
            Subject = "Test",
            Body = "Body",
            CustomHeaders = { { "X-Custom", "Value" } }
        };

        var message = MimeMessageBuilder.Build(email, "sender@example.com");

        message.Headers["X-Custom"].Should().Be("Value");
    }

    [Fact]
    public void Build_WithAttachment_CreatesMultipart()
    {
        var email = new EmailMessage
        {
            To = { "to@example.com" },
            Subject = "Test",
            Body = "Body",
            Attachments =
            {
                new EmailAttachment
                {
                    FileName = "test.txt",
                    ContentType = "text/plain",
                    Content = "Hello World"u8.ToArray()
                }
            }
        };

        var message = MimeMessageBuilder.Build(email, "sender@example.com");

        message.Body.Should().BeOfType<Multipart>();
        var multipart = (Multipart)message.Body;
        multipart.Count.Should().Be(2); // Body + Attachment
    }

    [Fact]
    public void ToBase64UrlString_EncodesMessage()
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress("Test", "test@example.com"));
        message.To.Add(new MailboxAddress("Recipient", "recipient@example.com"));
        message.Subject = "Test";
        message.Body = new TextPart("plain") { Text = "Hello" };

        var encoded = MimeMessageBuilder.ToBase64UrlString(message);

        encoded.Should().NotBeNullOrEmpty();
        encoded.Should().NotContain("+");
        encoded.Should().NotContain("/");
        encoded.Should().NotEndWith("=");
    }
}
