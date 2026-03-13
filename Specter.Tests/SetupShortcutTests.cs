using Xunit;
using static Specter.Wildcard;

namespace Specter.Tests;

/// <summary>
/// Tests for the shortcut setup syntax: mock.Method(matchers).Returns(value)
/// instead of: mock.Setup(x => x.Method(matchers)).Returns(value)
/// </summary>
public class SetupShortcutTests
{
    [Fact]
    public void Shortcut_with_wildcard_returns_value()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);

        Assert.True(mock.Instance.Send("a@b.com", "hi"));
    }

    [Fact]
    public void Shortcut_with_exact_first_arg()
    {
        var mock = new MockEmailService();
        mock.Send("admin@site.com", Any).Returns(true);

        Assert.True(mock.Instance.Send("admin@site.com", "anything"));
        Assert.False(mock.Instance.Send("other@site.com", "anything"));
    }

    [Fact]
    public void Shortcut_last_wins_over_earlier()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);
        mock.Send("blocked@evil.com", Any).Returns(false);

        Assert.False(mock.Instance.Send("blocked@evil.com", "x"));
        Assert.True(mock.Instance.Send("legit@good.com", "x"));
    }

    [Fact]
    public void Shortcut_and_setup_lambda_are_equivalent()
    {
        var mockA = new MockEmailService();
        mockA.Send(Any, Any).Returns(true);

        var mockB = new MockEmailService();
        mockB.Setup(x => x.Send(Any, Any)).Returns(true);

        Assert.Equal(mockA.Instance.Send("a@b.com", "hi"), mockB.Instance.Send("a@b.com", "hi"));
    }

    [Fact]
    public void Shortcut_verify_still_works()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);
        mock.Instance.Send("a@b.com", "hi");

        mock.Verify(x => x.Send(Any, Any), Times.Once);
    }

    [Fact]
    public void Shortcut_for_string_returning_method()
    {
        var mock = new MockEmailService();
        mock.GetTemplate(Any, Any).Returns("my-template");

        Assert.Equal("my-template", mock.Instance.GetTemplate("welcome", 1));
    }

    [Fact]
    public void Shortcut_for_void_method()
    {
        var repo = new MockUserRepository();
        int called = 0;
        repo.Delete(Any).Callback(() => called++);

        repo.Instance.Delete(42);

        Assert.Equal(1, called);
    }

    [Fact]
    public void Shortcut_mixed_with_setup_lambda_in_same_test()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);                          // shortcut
        mock.Setup(x => x.GetTemplate("v2", Any)).Returns("v2!");  // lambda

        Assert.True(mock.Instance.Send("a@b.com", "hi"));
        Assert.Equal("v2!", mock.Instance.GetTemplate("v2", 1));
    }

    [Fact]
    public void Concrete_call_with_real_args_uses_interface_impl_not_shortcut()
    {
        // Verifies overload resolution: string → string beats string → Matcher<string>
        var mock = new MockEmailService();
        mock.Send("specific@b.com", Any).Returns(true);

        // Calling Instance with real strings hits the interface impl, which dispatches via interceptor
        Assert.True(mock.Instance.Send("specific@b.com", "hi"));
        Assert.False(mock.Instance.Send("other@b.com", "hi"));
    }
}
