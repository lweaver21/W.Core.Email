using System.Collections.Concurrent;

namespace W.Core.Email.Templating;

/// <summary>
/// Thread-safe implementation of the template registry.
/// </summary>
public class TemplateRegistry : ITemplateRegistry
{
    private readonly ConcurrentDictionary<TemplateKey, IEmailTemplate> _templates = new();

    /// <inheritdoc />
    public void Register(string projectKey, string templateType, IEmailTemplate template)
    {
        if (string.IsNullOrWhiteSpace(projectKey))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(projectKey));
        if (string.IsNullOrWhiteSpace(templateType))
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(templateType));
        if (template is null)
            throw new ArgumentNullException(nameof(template));

        var key = new TemplateKey(projectKey, templateType);
        _templates[key] = template;
    }

    /// <inheritdoc />
    public IEmailTemplate Get(string projectKey, string templateType)
    {
        var key = new TemplateKey(projectKey, templateType);
        if (_templates.TryGetValue(key, out var template))
        {
            return template;
        }
        throw new KeyNotFoundException($"Template '{templateType}' not found for project '{projectKey}'.");
    }

    /// <inheritdoc />
    public bool TryGet(string projectKey, string templateType, out IEmailTemplate? template)
    {
        var key = new TemplateKey(projectKey, templateType);
        var found = _templates.TryGetValue(key, out var result);
        template = result;
        return found;
    }

    /// <inheritdoc />
    public bool HasTemplate(string projectKey, string templateType)
    {
        var key = new TemplateKey(projectKey, templateType);
        return _templates.ContainsKey(key);
    }

    /// <inheritdoc />
    public IEnumerable<string> GetRegisteredProjects()
    {
        return _templates.Keys.Select(k => k.ProjectKey).Distinct();
    }

    /// <inheritdoc />
    public IEnumerable<string> GetTemplateTypes(string projectKey)
    {
        return _templates.Keys
            .Where(k => k.ProjectKey == projectKey)
            .Select(k => k.TemplateType);
    }

    /// <inheritdoc />
    public bool Remove(string projectKey, string templateType)
    {
        var key = new TemplateKey(projectKey, templateType);
        return _templates.TryRemove(key, out _);
    }
}
