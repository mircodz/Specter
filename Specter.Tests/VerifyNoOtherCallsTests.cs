using Xunit;
using static Specter.Wildcard;

namespace Specter.Tests;

public class VerifyNoOtherCallsTests
{
    [Fact]
    public void Passes_when_no_calls_made()
    {
        var mock = new MockEmailService();
        mock.VerifyNoOtherCalls();
    }

    [Fact]
    public void Passes_when_all_calls_verified()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Any, Any)).Returns(true);
        mock.Instance.Send("a@b.com", "hi");

        mock.Verify(x => x.Send(Any, Any), Times.Once);
        mock.VerifyNoOtherCalls();
    }

    [Fact]
    public void Fails_when_call_was_not_verified()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Any, Any)).Returns(true);
        mock.Instance.Send("a@b.com", "hi");

        Assert.Throws<VerificationException>(() => mock.VerifyNoOtherCalls());
    }

    [Fact]
    public void Fails_when_second_call_not_verified()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Any, Any)).Returns(true);
        mock.Instance.Send("a@b.com", "hi");
        mock.Instance.Send("b@b.com", "hi");

        mock.Verify(x => x.Send("a@b.com", Any), Times.Once);

        // second Send was not covered by that Verify
        Assert.Throws<VerificationException>(() => mock.VerifyNoOtherCalls());
    }

    [Fact]
    public void Multiple_verifies_cover_multiple_calls()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Any, Any)).Returns(true);
        mock.Instance.Send("a@b.com", "hi");
        mock.Instance.Send("b@b.com", "hi");

        mock.Verify(x => x.Send("a@b.com", Any), Times.Once);
        mock.Verify(x => x.Send("b@b.com", Any), Times.Once);
        mock.VerifyNoOtherCalls();
    }

    [Fact]
    public void Wildcard_verify_covers_all_matching_calls()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Any, Any)).Returns(true);
        mock.Instance.Send("a@b.com", "hi");
        mock.Instance.Send("b@b.com", "hello");

        mock.Verify(x => x.Send(Any, Any), Times.Exactly(2));
        mock.VerifyNoOtherCalls();
    }

    [Fact]
    public void Different_methods_each_need_verification()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Any, Any)).Returns(true);
        mock.Setup(x => x.GetTemplate(Any, Any)).Returns("t");
        mock.Instance.Send("a@b.com", "hi");
        mock.Instance.GetTemplate("welcome", 1);

        mock.Verify(x => x.Send(Any, Any), Times.Once);
        // GetTemplate not verified
        Assert.Throws<VerificationException>(() => mock.VerifyNoOtherCalls());
    }

    [Fact]
    public void Reset_clears_verified_state()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Any, Any)).Returns(true);
        mock.Instance.Send("a@b.com", "hi");
        mock.Verify(x => x.Send(Any, Any), Times.Once);

        mock.Reset();

        // After reset, no calls — should pass
        mock.VerifyNoOtherCalls();
    }
}
