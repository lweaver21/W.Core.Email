using FluentAssertions;
using W.Core.Email.Models;
using W.Core.Email.Templating;
using Xunit;

namespace W.Core.Email.Tests.Templating;

public class TemplateRegistryTests
{
    private readonly TemplateRegistry _registry = new();

    [Fact]
    public void Register_ValidTemplate_Succeeds()
    {
        var template = new EmailTemplate
        {
            Subject = "Test",
            BodyTemplate = "Body"
        };

        _registry.Register("ProjectA", "WelcomeEmail", template);

        _registry.HasTemplate("ProjectA", "WelcomeEmail").Should().BeTrue();
    }

    [Fact]
    public void Register_EmptyProjectKey_ThrowsArgumentException()
    {
        var template = new EmailTemplate { Subject = "Test", BodyTemplate = "Body" };

        var act = () => _registry.Register("", "WelcomeEmail", template);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Register_EmptyTemplateType_ThrowsArgumentException()
    {
        var template = new EmailTemplate { Subject = "Test", BodyTemplate = "Body" };

        var act = () => _registry.Register("ProjectA", "", template);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Register_NullTemplate_ThrowsArgumentNullException()
    {
        var act = () => _registry.Register("ProjectA", "WelcomeEmail", null!);

        act.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void Get_ExistingTemplate_ReturnsTemplate()
    {
        var template = new EmailTemplate
        {
            Subject = "Welcome {{UserName}}",
            BodyTemplate = "Hello!"
        };
        _registry.Register("ProjectA", "WelcomeEmail", template);

        var result = _registry.Get("ProjectA", "WelcomeEmail");

        result.Should().BeSameAs(template);
    }

    [Fact]
    public void Get_NonExistingTemplate_ThrowsKeyNotFoundException()
    {
        var act = () => _registry.Get("ProjectA", "NonExistent");

        act.Should().Throw<KeyNotFoundException>()
            .WithMessage("*NonExistent*ProjectA*");
    }

    [Fact]
    public void TryGet_ExistingTemplate_ReturnsTrueAndTemplate()
    {
        var template = new EmailTemplate { Subject = "Test", BodyTemplate = "Body" };
        _registry.Register("ProjectA", "WelcomeEmail", template);

        var success = _registry.TryGet("ProjectA", "WelcomeEmail", out var result);

        success.Should().BeTrue();
        result.Should().BeSameAs(template);
    }

    [Fact]
    public void TryGet_NonExistingTemplate_ReturnsFalse()
    {
        var success = _registry.TryGet("ProjectA", "NonExistent", out var result);

        success.Should().BeFalse();
        result.Should().BeNull();
    }

    [Fact]
    public void HasTemplate_Existing_ReturnsTrue()
    {
        var template = new EmailTemplate { Subject = "Test", BodyTemplate = "Body" };
        _registry.Register("ProjectA", "WelcomeEmail", template);

        _registry.HasTemplate("ProjectA", "WelcomeEmail").Should().BeTrue();
    }

    [Fact]
    public void HasTemplate_NonExisting_ReturnsFalse()
    {
        _registry.HasTemplate("ProjectA", "NonExistent").Should().BeFalse();
    }

    [Fact]
    public void GetRegisteredProjects_ReturnsDistinctProjects()
    {
        _registry.Register("ProjectA", "Template1", new EmailTemplate { Subject = "T", BodyTemplate = "B" });
        _registry.Register("ProjectA", "Template2", new EmailTemplate { Subject = "T", BodyTemplate = "B" });
        _registry.Register("ProjectB", "Template1", new EmailTemplate { Subject = "T", BodyTemplate = "B" });

        var projects = _registry.GetRegisteredProjects().ToList();

        projects.Should().HaveCount(2);
        projects.Should().Contain("ProjectA");
        projects.Should().Contain("ProjectB");
    }

    [Fact]
    public void GetTemplateTypes_ReturnsTypesForProject()
    {
        _registry.Register("ProjectA", "Welcome", new EmailTemplate { Subject = "T", BodyTemplate = "B" });
        _registry.Register("ProjectA", "Reset", new EmailTemplate { Subject = "T", BodyTemplate = "B" });
        _registry.Register("ProjectB", "Other", new EmailTemplate { Subject = "T", BodyTemplate = "B" });

        var types = _registry.GetTemplateTypes("ProjectA").ToList();

        types.Should().HaveCount(2);
        types.Should().Contain("Welcome");
        types.Should().Contain("Reset");
        types.Should().NotContain("Other");
    }

    [Fact]
    public void Remove_ExistingTemplate_ReturnsTrue()
    {
        _registry.Register("ProjectA", "WelcomeEmail", new EmailTemplate { Subject = "T", BodyTemplate = "B" });

        var removed = _registry.Remove("ProjectA", "WelcomeEmail");

        removed.Should().BeTrue();
        _registry.HasTemplate("ProjectA", "WelcomeEmail").Should().BeFalse();
    }

    [Fact]
    public void Remove_NonExistingTemplate_ReturnsFalse()
    {
        var removed = _registry.Remove("ProjectA", "NonExistent");

        removed.Should().BeFalse();
    }

    [Fact]
    public void MultiProject_Isolation_Works()
    {
        var templateA = new EmailTemplate { Subject = "Welcome to A", BodyTemplate = "A Body" };
        var templateB = new EmailTemplate { Subject = "Welcome to B", BodyTemplate = "B Body" };

        _registry.Register("ProjectA", "Welcome", templateA);
        _registry.Register("ProjectB", "Welcome", templateB);

        var resultA = _registry.Get("ProjectA", "Welcome");
        var resultB = _registry.Get("ProjectB", "Welcome");

        resultA.Subject.Should().Be("Welcome to A");
        resultB.Subject.Should().Be("Welcome to B");
        resultA.Should().NotBeSameAs(resultB);
    }
}
