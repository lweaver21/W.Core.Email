namespace W.Core.Email.Templating;

/// <summary>
/// A unique key identifying a template within a project.
/// </summary>
/// <param name="ProjectKey">The project identifier.</param>
/// <param name="TemplateType">The template type name.</param>
public readonly record struct TemplateKey(string ProjectKey, string TemplateType)
{
    /// <summary>
    /// Returns a string representation of the template key.
    /// </summary>
    public override string ToString() => $"{ProjectKey}:{TemplateType}";

    /// <summary>
    /// Creates a TemplateKey from a combined string (e.g., "ProjectA:WelcomeEmail").
    /// </summary>
    /// <param name="combined">The combined string.</param>
    /// <returns>A new TemplateKey.</returns>
    /// <exception cref="ArgumentException">Thrown when the format is invalid.</exception>
    public static TemplateKey Parse(string combined)
    {
        var parts = combined.Split(':');
        if (parts.Length != 2)
        {
            throw new ArgumentException($"Invalid template key format: '{combined}'. Expected 'ProjectKey:TemplateType'.", nameof(combined));
        }
        return new TemplateKey(parts[0], parts[1]);
    }

    /// <summary>
    /// Tries to parse a combined string into a TemplateKey.
    /// </summary>
    /// <param name="combined">The combined string.</param>
    /// <param name="result">The resulting TemplateKey if successful.</param>
    /// <returns>True if parsing succeeded, false otherwise.</returns>
    public static bool TryParse(string combined, out TemplateKey result)
    {
        result = default;
        var parts = combined.Split(':');
        if (parts.Length != 2)
        {
            return false;
        }
        result = new TemplateKey(parts[0], parts[1]);
        return true;
    }
}
