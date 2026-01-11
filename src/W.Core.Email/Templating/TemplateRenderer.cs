using System.Reflection;
using System.Text.RegularExpressions;

namespace W.Core.Email.Templating;

/// <summary>
/// Renders email templates by replacing placeholders with model values.
/// </summary>
public class TemplateRenderer
{
    private static readonly Regex PlaceholderPattern = new(
        @"\{\{(\w+)(?::([^}]+))?\}\}",
        RegexOptions.Compiled);

    /// <summary>
    /// Renders a template string with the given model.
    /// </summary>
    /// <param name="template">The template string containing {{placeholder}} markers.</param>
    /// <param name="model">The model object containing values for placeholders.</param>
    /// <returns>The rendered string with placeholders replaced.</returns>
    public string Render(string template, object? model)
    {
        if (model is null)
        {
            return template;
        }

        var properties = GetModelProperties(model);

        return PlaceholderPattern.Replace(template, match =>
        {
            var key = match.Groups[1].Value;
            var format = match.Groups[2].Success ? match.Groups[2].Value : null;

            if (properties.TryGetValue(key, out var value))
            {
                return FormatValue(value, format);
            }

            // Leave unmatched placeholders as-is
            return match.Value;
        });
    }

    /// <summary>
    /// Renders both subject and body of a template.
    /// </summary>
    /// <param name="template">The email template.</param>
    /// <param name="model">The model object.</param>
    /// <returns>A tuple containing the rendered subject and body.</returns>
    public (string Subject, string Body) RenderTemplate(IEmailTemplate template, object? model)
    {
        var subject = Render(template.Subject, model);
        var body = Render(template.BodyTemplate, model);
        return (subject, body);
    }

    private static Dictionary<string, object?> GetModelProperties(object model)
    {
        var result = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase);

        // Handle dictionary
        if (model is IDictionary<string, object?> dict)
        {
            foreach (var kvp in dict)
            {
                result[kvp.Key] = kvp.Value;
            }
            return result;
        }

        // Handle anonymous types and regular objects
        var type = model.GetType();
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
        {
            if (prop.CanRead)
            {
                result[prop.Name] = prop.GetValue(model);
            }
        }

        return result;
    }

    private static string FormatValue(object? value, string? format)
    {
        if (value is null)
        {
            return string.Empty;
        }

        if (format is not null && value is IFormattable formattable)
        {
            return formattable.ToString(format, null) ?? string.Empty;
        }

        return value.ToString() ?? string.Empty;
    }
}
