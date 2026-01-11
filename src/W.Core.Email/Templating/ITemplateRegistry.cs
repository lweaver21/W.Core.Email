namespace W.Core.Email.Templating;

/// <summary>
/// Registry for managing email templates across projects.
/// </summary>
public interface ITemplateRegistry
{
    /// <summary>
    /// Registers a template for a project.
    /// </summary>
    /// <param name="projectKey">The project identifier.</param>
    /// <param name="templateType">The template type name.</param>
    /// <param name="template">The template to register.</param>
    void Register(string projectKey, string templateType, IEmailTemplate template);

    /// <summary>
    /// Gets a template by project and type.
    /// </summary>
    /// <param name="projectKey">The project identifier.</param>
    /// <param name="templateType">The template type name.</param>
    /// <returns>The template if found.</returns>
    /// <exception cref="KeyNotFoundException">Thrown when the template is not found.</exception>
    IEmailTemplate Get(string projectKey, string templateType);

    /// <summary>
    /// Tries to get a template by project and type.
    /// </summary>
    /// <param name="projectKey">The project identifier.</param>
    /// <param name="templateType">The template type name.</param>
    /// <param name="template">The template if found.</param>
    /// <returns>True if the template was found, false otherwise.</returns>
    bool TryGet(string projectKey, string templateType, out IEmailTemplate? template);

    /// <summary>
    /// Checks if a template exists.
    /// </summary>
    /// <param name="projectKey">The project identifier.</param>
    /// <param name="templateType">The template type name.</param>
    /// <returns>True if the template exists.</returns>
    bool HasTemplate(string projectKey, string templateType);

    /// <summary>
    /// Gets all registered project keys.
    /// </summary>
    /// <returns>An enumerable of project keys.</returns>
    IEnumerable<string> GetRegisteredProjects();

    /// <summary>
    /// Gets all template types registered for a project.
    /// </summary>
    /// <param name="projectKey">The project identifier.</param>
    /// <returns>An enumerable of template types.</returns>
    IEnumerable<string> GetTemplateTypes(string projectKey);

    /// <summary>
    /// Removes a template from the registry.
    /// </summary>
    /// <param name="projectKey">The project identifier.</param>
    /// <param name="templateType">The template type name.</param>
    /// <returns>True if the template was removed, false if it didn't exist.</returns>
    bool Remove(string projectKey, string templateType);
}
