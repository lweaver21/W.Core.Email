using FluentAssertions;
using W.Core.Email.Templating;
using Xunit;

namespace W.Core.Email.Tests.Templating;

public class TemplateRendererTests
{
    private readonly TemplateRenderer _renderer = new();

    [Fact]
    public void Render_SimplePlaceholder_ReplacesValue()
    {
        var template = "Hello {{UserName}}!";
        var model = new { UserName = "John" };

        var result = _renderer.Render(template, model);

        result.Should().Be("Hello John!");
    }

    [Fact]
    public void Render_MultiplePlaceholders_ReplacesAll()
    {
        var template = "Hello {{FirstName}} {{LastName}}!";
        var model = new { FirstName = "John", LastName = "Doe" };

        var result = _renderer.Render(template, model);

        result.Should().Be("Hello John Doe!");
    }

    [Fact]
    public void Render_UnmatchedPlaceholder_KeepsOriginal()
    {
        var template = "Hello {{UserName}} from {{Company}}!";
        var model = new { UserName = "John" };

        var result = _renderer.Render(template, model);

        result.Should().Be("Hello John from {{Company}}!");
    }

    [Fact]
    public void Render_NullModel_ReturnsOriginalTemplate()
    {
        var template = "Hello {{UserName}}!";

        var result = _renderer.Render(template, null);

        result.Should().Be("Hello {{UserName}}!");
    }

    [Fact]
    public void Render_CaseInsensitive_ReplacesValue()
    {
        var template = "Hello {{username}}!";
        var model = new { UserName = "John" };

        var result = _renderer.Render(template, model);

        result.Should().Be("Hello John!");
    }

    [Fact]
    public void Render_FormattedDate_AppliesFormat()
    {
        var template = "Today is {{Date:yyyy-MM-dd}}";
        var model = new { Date = new DateTime(2024, 1, 15) };

        var result = _renderer.Render(template, model);

        result.Should().Be("Today is 2024-01-15");
    }

    [Fact]
    public void Render_FormattedNumber_AppliesFormat()
    {
        var template = "Total: {{Amount:C2}}";
        var model = new { Amount = 1234.56m };

        var result = _renderer.Render(template, model);

        result.Should().Contain("1");  // Currency formatting varies by culture
    }

    [Fact]
    public void Render_NullValue_ReturnsEmpty()
    {
        var template = "Hello {{UserName}}!";
        var model = new { UserName = (string?)null };

        var result = _renderer.Render(template, model);

        result.Should().Be("Hello !");
    }

    [Fact]
    public void Render_Dictionary_ReplacesValues()
    {
        var template = "Hello {{UserName}} from {{Company}}!";
        var model = new Dictionary<string, object?>
        {
            ["UserName"] = "John",
            ["Company"] = "Acme"
        };

        var result = _renderer.Render(template, model);

        result.Should().Be("Hello John from Acme!");
    }

    [Fact]
    public void RenderTemplate_RendersSubjectAndBody()
    {
        var template = new EmailTemplate
        {
            Subject = "Welcome {{UserName}}",
            BodyTemplate = "<h1>Hello {{UserName}}</h1><p>Welcome to {{AppName}}!</p>"
        };
        var model = new { UserName = "John", AppName = "MyApp" };

        var (subject, body) = _renderer.RenderTemplate(template, model);

        subject.Should().Be("Welcome John");
        body.Should().Be("<h1>Hello John</h1><p>Welcome to MyApp!</p>");
    }

    [Fact]
    public void Render_NoPlaceholders_ReturnsOriginal()
    {
        var template = "Hello World!";
        var model = new { UserName = "John" };

        var result = _renderer.Render(template, model);

        result.Should().Be("Hello World!");
    }

    [Fact]
    public void Render_EmptyTemplate_ReturnsEmpty()
    {
        var result = _renderer.Render("", new { UserName = "John" });

        result.Should().BeEmpty();
    }

    [Fact]
    public void Render_ComplexObject_ExtractsProperties()
    {
        var template = "Order #{{OrderId}} for {{Customer}}";
        var model = new
        {
            OrderId = 12345,
            Customer = "John Doe",
            IgnoredField = new { Nested = "value" }
        };

        var result = _renderer.Render(template, model);

        result.Should().Be("Order #12345 for John Doe");
    }
}
