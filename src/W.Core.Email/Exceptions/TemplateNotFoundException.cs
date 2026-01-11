namespace W.Core.Email.Exceptions;

/// <summary>
/// Exception thrown when a template is not found in the registry.
/// </summary>
public class TemplateNotFoundException : EmailException
{
    /// <summary>
    /// The project key that was searched.
    /// </summary>
    public string ProjectKey { get; }

    /// <summary>
    /// The template type that was not found.
    /// </summary>
    public string TemplateType { get; }

    /// <summary>
    /// Creates a new TemplateNotFoundException.
    /// </summary>
    /// <param name="projectKey">The project key.</param>
    /// <param name="templateType">The template type.</param>
    public TemplateNotFoundException(string projectKey, string templateType)
        : base($"Template '{templateType}' not found for project '{projectKey}'.", EmailErrorCode.TemplateNotFound)
    {
        ProjectKey = projectKey;
        TemplateType = templateType;
    }
}
