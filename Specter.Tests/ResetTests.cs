using Xunit;
using static Specter.Wildcard;

namespace Specter.Tests;

public class ResetTests
{
    [Fact]
    public void Reset_clears_setups()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Any, Any)).Returns(true);
        Assert.True(mock.Instance.Send("a@b.com", "hi"));

        mock.Reset();

        Assert.False(mock.Instance.Send("a@b.com", "hi"));
    }

    [Fact]
    public void Reset_clears_call_history()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Any, Any)).Returns(true);
        mock.Instance.Send("a@b.com", "hi");

        mock.Reset();

        mock.Verify(x => x.Send(Any, Any), Times.Never);
    }

    [Fact]
    public void Setups_added_after_reset_work_normally()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Any, Any)).Returns(true);
        mock.Reset();

        mock.Setup(x => x.Send(Any, Any)).Returns(false);

        Assert.False(mock.Instance.Send("a@b.com", "hi"));
    }
}