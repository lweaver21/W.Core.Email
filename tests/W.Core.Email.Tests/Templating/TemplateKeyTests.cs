using FluentAssertions;
using W.Core.Email.Templating;
using Xunit;

namespace W.Core.Email.Tests.Templating;

public class TemplateKeyTests
{
    [Fact]
    public void ToString_ReturnsCorrectFormat()
    {
        var key = new TemplateKey("ProjectA", "WelcomeEmail");

        key.ToString().Should().Be("ProjectA:WelcomeEmail");
    }

    [Fact]
    public void Parse_ValidString_ReturnsTemplateKey()
    {
        var key = TemplateKey.Parse("ProjectA:WelcomeEmail");

        key.ProjectKey.Should().Be("ProjectA");
        key.TemplateType.Should().Be("WelcomeEmail");
    }

    [Theory]
    [InlineData("InvalidFormat")]
    [InlineData("Too:Many:Colons")]
    [InlineData("")]
    public void Parse_InvalidString_ThrowsArgumentException(string input)
    {
        var act = () => TemplateKey.Parse(input);

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void TryParse_ValidString_ReturnsTrue()
    {
        var success = TemplateKey.TryParse("ProjectA:WelcomeEmail", out var key);

        success.Should().BeTrue();
        key.ProjectKey.Should().Be("ProjectA");
        key.TemplateType.Should().Be("WelcomeEmail");
    }

    [Fact]
    public void TryParse_InvalidString_ReturnsFalse()
    {
        var success = TemplateKey.TryParse("InvalidFormat", out var key);

        success.Should().BeFalse();
        key.Should().Be(default(TemplateKey));
    }

    [Fact]
    public void Equality_SameValues_AreEqual()
    {
        var key1 = new TemplateKey("ProjectA", "WelcomeEmail");
        var key2 = new TemplateKey("ProjectA", "WelcomeEmail");

        key1.Should().Be(key2);
        (key1 == key2).Should().BeTrue();
    }

    [Fact]
    public void Equality_DifferentValues_AreNotEqual()
    {
        var key1 = new TemplateKey("ProjectA", "WelcomeEmail");
        var key2 = new TemplateKey("ProjectB", "WelcomeEmail");

        key1.Should().NotBe(key2);
    }
}
