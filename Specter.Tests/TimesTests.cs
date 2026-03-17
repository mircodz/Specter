using Xunit;
using static Specter.Wildcard;

namespace Specter.Tests;

public class TimesTests
{
    [Fact]
    public void Times_Once_passes_when_called_once()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);
        mock.Instance.Send("a@b.com", "hi");

        mock.Send(Any, Any).Verify(Times.Once);
    }

    [Fact]
    public void Times_Once_fails_when_called_twice()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);
        mock.Instance.Send("a@b.com", "hi");
        mock.Instance.Send("b@b.com", "hi");

        Assert.Throws<VerificationException>(() => mock.Send(Any, Any).Verify(Times.Once));
    }

    [Fact]
    public void Times_Never_passes_when_not_called()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Verify(Times.Never);
    }

    [Fact]
    public void Times_Never_fails_when_called()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);
        mock.Instance.Send("a@b.com", "hi");

        Assert.Throws<VerificationException>(() => mock.Send(Any, Any).Verify(Times.Never));
    }

    [Fact]
    public void Times_AtLeastOnce_passes_when_called_multiple_times()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);
        mock.Instance.Send("a@b.com", "hi");
        mock.Instance.Send("b@b.com", "hi");

        mock.Send(Any, Any).Verify(Times.AtLeastOnce);
    }

    [Fact]
    public void Times_AtLeastOnce_fails_when_never_called()
    {
        var mock = new MockEmailService();
        Assert.Throws<VerificationException>(() => mock.Send(Any, Any).Verify(Times.AtLeastOnce));
    }

    [Fact]
    public void Times_Exactly_passes()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);
        mock.Instance.Send("a@b.com", "hi");
        mock.Instance.Send("b@b.com", "hi");
        mock.Instance.Send("c@b.com", "hi");

        mock.Send(Any, Any).Verify(Times.Exactly(3));
    }

    [Fact]
    public void Times_AtLeast_passes_when_count_is_sufficient()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);
        mock.Instance.Send("a@b.com", "hi");
        mock.Instance.Send("b@b.com", "hi");

        mock.Send(Any, Any).Verify(Times.AtLeast(2));
    }

    [Fact]
    public void Times_AtMost_passes_when_under_limit()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);
        mock.Instance.Send("a@b.com", "hi");

        mock.Send(Any, Any).Verify(Times.AtMost(3));
    }

    [Fact]
    public void Times_AtMost_fails_when_over_limit()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);
        mock.Instance.Send("a@b.com", "hi");
        mock.Instance.Send("b@b.com", "hi");
        mock.Instance.Send("c@b.com", "hi");
        mock.Instance.Send("d@b.com", "hi");

        Assert.Throws<VerificationException>(() => mock.Send(Any, Any).Verify(Times.AtMost(3)));
    }

    [Fact]
    public void Times_Between_passes_when_in_range()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);
        mock.Instance.Send("a@b.com", "hi");
        mock.Instance.Send("b@b.com", "hi");
        mock.Instance.Send("c@b.com", "hi");

        mock.Send(Any, Any).Verify(Times.Between(2, 4));
    }

    [Fact]
    public void Times_Between_fails_when_outside_range()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);
        mock.Instance.Send("a@b.com", "hi");

        Assert.Throws<VerificationException>(() => mock.Send(Any, Any).Verify(Times.Between(2, 4)));
    }

    [Fact]
    public void Times_works_with_void_methods()
    {
        var repo = new MockUserRepository();
        repo.Instance.Delete(1);
        repo.Instance.Delete(2);

        repo.Delete(Any).Verify(Times.Exactly(2));
    }
}
