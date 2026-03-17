using Xunit;
using static Specter.Wildcard;

namespace Specter.Tests;

public class WrappingTests
{
    [Fact]
    public void Unconfigured_calls_delegate_to_real_object()
    {
        var mock = new MockEmailService(wrapping: new RealEmailService());

        // No setup - should call through to RealEmailService
        Assert.Equal("real:welcome-v1", mock.Instance.GetTemplate("welcome", 1));
        Assert.True(mock.Instance.Send("a@b.com", "hi"));
    }

    [Fact]
    public void Setup_takes_priority_over_wrapped_object()
    {
        var mock = new MockEmailService(wrapping: new RealEmailService());
        mock.GetTemplate("welcome", 1).Returns("mocked");

        Assert.Equal("mocked", mock.Instance.GetTemplate("welcome", 1));
        // Unmatched call still delegates to real
        Assert.Equal("real:reset-v2", mock.Instance.GetTemplate("reset", 2));
    }

    [Fact]
    public void Calls_to_real_object_are_still_recorded()
    {
        var mock = new MockEmailService(wrapping: new RealEmailService());

        mock.Instance.Send("a@b.com", "hi");

        mock.Send(Any, Any).Verify(Times.Once);
    }

    [Fact]
    public void Callbacks_still_fire_on_matched_setups()
    {
        var mock = new MockEmailService(wrapping: new RealEmailService());
        var called = false;
        mock.Send(Any, Any).Callback(() => called = true);

        mock.Instance.Send("a@b.com", "hi");

        Assert.True(called);
    }
}
