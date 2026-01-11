using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace W.Core.Email.Configuration;

/// <summary>
/// Builder for configuring email services with a fluent API.
/// </summary>
public class EmailServiceBuilder
{
    /// <summary>
    /// The service collection being configured.
    /// </summary>
    public IServiceCollection Services { get; }

    /// <summary>
    /// The project key for this email configuration.
    /// </summary>
    public string ProjectKey { get; }

    internal EmailServiceBuilder(IServiceCollection services, string projectKey)
    {
        Services = services;
        ProjectKey = projectKey;
    }

    /// <summary>
    /// Configures Gmail as the email provider.
    /// </summary>
    /// <param name="configure">Action to configure Gmail options.</param>
    /// <returns>The builder for method chaining.</returns>
    public EmailServiceBuilder AddGmailProvider(Action<GmailAuthOptions> configure)
    {
        Services.Configure(configure);
        Services.AddSingleton<IValidateOptions<GmailAuthOptions>, GmailOptionsValidator>();
        return this;
    }
}
