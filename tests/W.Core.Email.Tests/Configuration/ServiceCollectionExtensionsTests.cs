using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using W.Core.Email.Configuration;
using Xunit;

namespace W.Core.Email.Tests.Configuration;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddEmail_WithAction_ConfiguresOptions()
    {
        var services = new ServiceCollection();

        var builder = services.AddEmail(options =>
        {
            options.ProjectKey = "TestProject";
            options.DefaultSenderName = "Test Sender";
        });

        builder.Should().NotBeNull();
        builder.ProjectKey.Should().Be("TestProject");

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<EmailOptions>>().Value;

        options.ProjectKey.Should().Be("TestProject");
        options.DefaultSenderName.Should().Be("Test Sender");
    }

    [Fact]
    public void AddEmail_RegistersValidator()
    {
        var services = new ServiceCollection();

        services.AddEmail(options =>
        {
            options.ProjectKey = "TestProject";
        });

        var provider = services.BuildServiceProvider();
        var validators = provider.GetServices<IValidateOptions<EmailOptions>>();

        validators.Should().ContainSingle()
            .Which.Should().BeOfType<EmailOptionsValidator>();
    }

    [Fact]
    public void AddGmailProvider_ConfiguresGmailOptions()
    {
        var services = new ServiceCollection();

        services.AddEmail(options =>
        {
            options.ProjectKey = "TestProject";
        })
        .AddGmailProvider(gmail =>
        {
            gmail.CredentialsPath = "/path/to/credentials.json";
            gmail.SenderEmail = "sender@example.com";
        });

        var provider = services.BuildServiceProvider();
        var options = provider.GetRequiredService<IOptions<GmailAuthOptions>>().Value;

        options.CredentialsPath.Should().Be("/path/to/credentials.json");
        options.SenderEmail.Should().Be("sender@example.com");
    }

    [Fact]
    public void AddGmailProvider_RegistersValidator()
    {
        var services = new ServiceCollection();

        services.AddEmail(options =>
        {
            options.ProjectKey = "TestProject";
        })
        .AddGmailProvider(gmail =>
        {
            gmail.CredentialsPath = "/path/to/credentials.json";
            gmail.SenderEmail = "sender@example.com";
        });

        var provider = services.BuildServiceProvider();
        var validators = provider.GetServices<IValidateOptions<GmailAuthOptions>>();

        validators.Should().ContainSingle()
            .Which.Should().BeOfType<GmailOptionsValidator>();
    }

    [Fact]
    public void AddEmail_FluentChaining_Works()
    {
        var services = new ServiceCollection();

        var builder = services
            .AddEmail(options =>
            {
                options.ProjectKey = "TestProject";
                options.DefaultSenderEmail = "default@example.com";
            })
            .AddGmailProvider(gmail =>
            {
                gmail.CredentialsPath = "/path/to/credentials.json";
                gmail.SenderEmail = "sender@example.com";
                gmail.AuthMethod = GmailAuthMethod.ServiceAccount;
            });

        builder.Services.Should().BeSameAs(services);

        var provider = services.BuildServiceProvider();

        var emailOptions = provider.GetRequiredService<IOptions<EmailOptions>>().Value;
        var gmailOptions = provider.GetRequiredService<IOptions<GmailAuthOptions>>().Value;

        emailOptions.ProjectKey.Should().Be("TestProject");
        gmailOptions.AuthMethod.Should().Be(GmailAuthMethod.ServiceAccount);
    }
}
