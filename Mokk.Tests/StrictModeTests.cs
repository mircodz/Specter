using Xunit;
using static Mokk.Wildcard;

namespace Mokk.Tests;

public class StrictModeTests
{
    [Fact]
    public void Throws_when_no_setup_matches()
    {
        var mock = new MockEmailService(strict: true);

        Assert.Throws<MissingSetupException>(() => mock.Instance.Send("a@b.com", "hi"));
    }

    [Fact]
    public void Does_not_throw_when_setup_matches()
    {
        var mock = new MockEmailService(strict: true);
        mock.Send(Any, Any).Returns(true);

        Assert.True(mock.Instance.Send("a@b.com", "hi"));
    }

    [Fact]
    public void Non_strict_returns_default_when_no_setup()
    {
        var mock = new MockEmailService();

        Assert.False(mock.Instance.Send("a@b.com", "hi"));
    }
}
