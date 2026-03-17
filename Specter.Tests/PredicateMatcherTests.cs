using Xunit;
using static Specter.Wildcard;

namespace Specter.Tests;

public class PredicateMatcherTests
{
    [Fact]
    public void Predicate_matcher_with_Matcher_Is()
    {
        var mock = new MockEmailService();
        mock.Send(
            Matcher<string>.Is(s => s.EndsWith("@internal.com"), "@internal.com"),
            Any
        ).Returns(true);

        Assert.True(mock.Instance.Send("alice@internal.com", "hi"));
        Assert.False(mock.Instance.Send("alice@external.com", "hi"));
    }

    [Fact]
    public void Predicate_matcher_with_Arg_Is()
    {
        var mock = new MockEmailService();
        mock.Send(
            Arg.Is<string>(s => s.Contains("@")),
            Any
        ).Returns(true);

        Assert.True(mock.Instance.Send("test@example.com", "subject"));
        Assert.False(mock.Instance.Send("no-at-sign", "subject"));
    }

    [Fact]
    public void Arg_Any_matches_any_value()
    {
        var mock = new MockEmailService();
        mock.Send(Arg.Any<string>(), Arg.Any<string>()).Returns(true);

        Assert.True(mock.Instance.Send("any@any.com", "any"));
    }
}
