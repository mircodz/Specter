using Xunit;
using static Mokk.Wildcard;

namespace Mokk.Tests;

/// <summary>
/// Tests for the direct setup syntax: mock.Method(matchers).Returns(value)
/// </summary>
public class SetupShortcutTests
{
    [Fact]
    public void Wildcard_returns_value()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);

        Assert.True(mock.Instance.Send("a@b.com", "hi"));
    }

    [Fact]
    public void Exact_first_arg()
    {
        var mock = new MockEmailService();
        mock.Send("admin@site.com", Any).Returns(true);

        Assert.True(mock.Instance.Send("admin@site.com", "anything"));
        Assert.False(mock.Instance.Send("other@site.com", "anything"));
    }

    [Fact]
    public void Last_wins_over_earlier()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);
        mock.Send("blocked@evil.com", Any).Returns(false);

        Assert.False(mock.Instance.Send("blocked@evil.com", "x"));
        Assert.True(mock.Instance.Send("legit@good.com", "x"));
    }

    [Fact]
    public void Verify_after_setup()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);
        mock.Instance.Send("a@b.com", "hi");

        mock.Send(Any, Any).Verify(Times.Once);
    }

    [Fact]
    public void String_returning_method()
    {
        var mock = new MockEmailService();
        mock.GetTemplate(Any, Any).Returns("my-template");

        Assert.Equal("my-template", mock.Instance.GetTemplate("welcome", 1));
    }

    [Fact]
    public void Void_method_callback()
    {
        var repo = new MockUserRepository();
        int called = 0;
        repo.Delete(Any).Callback(() => called++);

        repo.Instance.Delete(42);

        Assert.Equal(1, called);
    }

    [Fact]
    public void Multiple_setups_on_different_methods()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);
        mock.GetTemplate("v2", Any).Returns("v2!");

        Assert.True(mock.Instance.Send("a@b.com", "hi"));
        Assert.Equal("v2!", mock.Instance.GetTemplate("v2", 1));
    }

    [Fact]
    public void Zero_param_method_shortcut_works()
    {
        var mock = new MockExtendedService();
        mock.GetName().Returns("Test");
        mock.GetCount().Returns(7);

        Assert.Equal("Test", mock.Instance.GetName());
        Assert.Equal(7, mock.Instance.GetCount());
    }
}
