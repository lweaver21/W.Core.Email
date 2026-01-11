using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using W.Core.Email.Services;
using W.Core.Email.Templating;

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

        // Ensure the template registry is registered as a singleton
        Services.TryAddSingleton<ITemplateRegistry, TemplateRegistry>();
        Services.TryAddSingleton<TemplateRenderer>();
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

        // Register Gmail client and email service
        Services.TryAddSingleton<IGmailClient, GmailClient>();
        Services.TryAddScoped<IEmailService, EmailService>();

        return this;
    }

    /// <summary>
    /// Registers an email template for this project.
    /// </summary>
    /// <param name="templateType">The template type name (e.g., "WelcomeEmail").</param>
    /// <param name="template">The template instance.</param>
    /// <returns>The builder for method chaining.</returns>
    public EmailServiceBuilder AddTemplate(string templateType, IEmailTemplate template)
    {
        if (string.IsNullOrWhiteSpace(templateType))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(templateType));
        if (template is null)
            throw new ArgumentNullException(nameof(template));

        var projectKey = ProjectKey;

        // Use PostConfigure to register templates after all services are configured
        Services.PostConfigure<TemplateRegistryOptions>(options =>
        {
            options.Templates.Add(new TemplateRegistration(projectKey, templateType, template));
        });

        // Add the initializer if not already added
        Services.TryAddSingleton<TemplateRegistryInitializer>();

        return this;
    }

    /// <summary>
    /// Registers a simple text email template.
    /// </summary>
    /// <param name="templateType">The template type name.</param>
    /// <param name="subject">The subject template.</param>
    /// <param name="body">The body template.</param>
    /// <returns>The builder for method chaining.</returns>
    public EmailServiceBuilder AddTextTemplate(string templateType, string subject, string body)
    {
        return AddTemplate(templateType, EmailTemplate.Text(subject, body));
    }

    /// <summary>
    /// Registers an HTML email template.
    /// </summary>
    /// <param name="templateType">The template type name.</param>
    /// <param name="subject">The subject template.</param>
    /// <param name="body">The HTML body template.</param>
    /// <returns>The builder for method chaining.</returns>
    public EmailServiceBuilder AddHtmlTemplate(string templateType, string subject, string body)
    {
        return AddTemplate(templateType, EmailTemplate.Html(subject, body));
    }
}

/// <summary>
/// Options for template registration.
/// </summary>
public class TemplateRegistryOptions
{
    /// <summary>
    /// Templates to register.
    /// </summary>
    public List<TemplateRegistration> Templates { get; } = new();
}

/// <summary>
/// Represents a template registration.
/// </summary>
public record TemplateRegistration(string ProjectKey, string TemplateType, IEmailTemplate Template);

/// <summary>
/// Initializes the template registry with configured templates.
/// </summary>
public class TemplateRegistryInitializer
{
    public TemplateRegistryInitializer(
        ITemplateRegistry registry,
        IOptions<TemplateRegistryOptions> options)
    {
        foreach (var registration in options.Value.Templates)
        {
            registry.Register(registration.ProjectKey, registration.TemplateType, registration.Template);
        }
    }
}
