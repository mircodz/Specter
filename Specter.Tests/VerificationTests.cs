using Xunit;
using static Specter.Wildcard;

namespace Specter.Tests;

public class VerificationTests
{
    [Fact]
    public void Exactly_N_calls()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Any, Any)).Returns(true);

        mock.Instance.Send("a@test.com", "s1");
        mock.Instance.Send("b@test.com", "s2");

        mock.Verify(x => x.Send(Any, Any), Times.Exactly(2));
    }

    [Fact]
    public void AtLeast_N_calls()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Any, Any)).Returns(true);

        mock.Instance.Send("a@test.com", "s1");
        mock.Instance.Send("b@test.com", "s2");
        mock.Instance.Send("c@test.com", "s3");

        mock.Verify(x => x.Send(Any, Any), Times.AtLeast(2));
    }

    [Fact]
    public void Fails_when_count_doesnt_match()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Any, Any)).Returns(true);
        mock.Instance.Send("a@test.com", "s1");

        Assert.Throws<VerificationException>(() =>
            mock.Verify(x => x.Send(Any, Any), Times.Exactly(2)));
    }

    [Fact]
    public void Scoped_to_specific_argument()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Any, Any)).Returns(true);

        mock.Instance.Send("admin@site.com", "s1");
        mock.Instance.Send("admin@site.com", "s2");
        mock.Instance.Send("other@site.com", "s3");

        mock.Verify(x => x.Send("admin@site.com", Any), Times.Exactly(2));
    }

    [Fact]
    public void Void_method_calls()
    {
        var mock = new MockUserRepository();
        mock.Instance.Delete(1);
        mock.Instance.Delete(2);

        mock.Verify(x => x.Delete(Any), Times.Exactly(2));
    }
}
