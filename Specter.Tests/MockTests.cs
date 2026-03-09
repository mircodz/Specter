using Xunit;
using static Specter.Wildcard;

namespace Specter.Tests;

public class WildcardMatchingTests
{
    [Fact]
    public void Wildcard_matches_any_argument()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Any, Any)).Returns(true);

        Assert.True(mock.Instance.Send("anyone@test.com", "Hello"));
        Assert.True(mock.Instance.Send("other@test.com", "World"));
    }

    [Fact]
    public void Exact_value_match_via_implicit_conversion()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send("admin@site.com", Any)).Returns(true);

        Assert.True(mock.Instance.Send("admin@site.com", "anything"));
        Assert.False(mock.Instance.Send("other@site.com", "anything"));
    }

    [Fact]
    public void Last_setup_wins_over_earlier_wildcard()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Any, Any)).Returns(true);
        mock.Setup(x => x.Send("blocked@evil.com", Any)).Returns(false);

        Assert.False(mock.Instance.Send("blocked@evil.com", "anything"));
        Assert.True(mock.Instance.Send("legit@good.com", "anything"));
    }

    [Fact]
    public void Mixed_types_int_version_with_wildcard_string()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.GetTemplate(Any, 2)).Returns("v2-template");

        Assert.Equal("v2-template", mock.Instance.GetTemplate("welcome", 2));
        Assert.Equal("v2-template", mock.Instance.GetTemplate("any-name", 2));
        Assert.Equal("", mock.Instance.GetTemplate("welcome", 3));
    }
}