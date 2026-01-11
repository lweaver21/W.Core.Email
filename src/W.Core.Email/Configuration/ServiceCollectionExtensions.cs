using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace W.Core.Email.Configuration;

/// <summary>
/// Extension methods for configuring email services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds email services to the service collection.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configure">Action to configure email options.</param>
    /// <returns>An EmailServiceBuilder for additional configuration.</returns>
    public static EmailServiceBuilder AddEmail(
        this IServiceCollection services,
        Action<EmailOptions> configure)
    {
        services.Configure(configure);
        services.AddSingleton<IValidateOptions<EmailOptions>, EmailOptionsValidator>();

        // Get the project key from options for the builder
        var options = new EmailOptions();
        configure(options);

        return new EmailServiceBuilder(services, options.ProjectKey);
    }

    /// <summary>
    /// Adds email services to the service collection from configuration.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration section containing email options.</param>
    /// <returns>An EmailServiceBuilder for additional configuration.</returns>
    public static EmailServiceBuilder AddEmail(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var section = configuration.GetSection(EmailOptions.SectionName);
        services.Configure<EmailOptions>(section);
        services.AddSingleton<IValidateOptions<EmailOptions>, EmailOptionsValidator>();

        var options = section.Get<EmailOptions>() ?? new EmailOptions();

        return new EmailServiceBuilder(services, options.ProjectKey);
    }

    /// <summary>
    /// Adds email services to the service collection with both action and configuration binding.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configuration">The configuration section for base settings.</param>
    /// <param name="configure">Action to override or add to configuration.</param>
    /// <returns>An EmailServiceBuilder for additional configuration.</returns>
    public static EmailServiceBuilder AddEmail(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<EmailOptions> configure)
    {
        var section = configuration.GetSection(EmailOptions.SectionName);
        services.Configure<EmailOptions>(section);
        services.PostConfigure(configure);
        services.AddSingleton<IValidateOptions<EmailOptions>, EmailOptionsValidator>();

        var options = section.Get<EmailOptions>() ?? new EmailOptions();
        configure(options);

        return new EmailServiceBuilder(services, options.ProjectKey);
    }
}
