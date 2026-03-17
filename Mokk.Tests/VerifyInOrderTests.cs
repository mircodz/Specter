using System.Threading.Tasks;
using Xunit;
using static Mokk.Wildcard;

namespace Mokk.Tests;

public class VerifyInOrderTests
{
    [Fact]
    public void Passes_when_calls_happen_in_order()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);

        mock.Instance.GetTemplate("welcome", 1);
        mock.Instance.Send("a@b.com", "hi");

        mock.VerifyInOrder(
            mock.GetTemplate(Any, Any),
            mock.Send(Any, Any)
        );
    }

    [Fact]
    public void Fails_when_calls_happen_in_wrong_order()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);

        mock.Instance.Send("a@b.com", "hi");
        mock.Instance.GetTemplate("welcome", 1);

        Assert.Throws<VerificationException>(() =>
            mock.VerifyInOrder(
                mock.GetTemplate(Any, Any),
                mock.Send(Any, Any)
            ));
    }

    [Fact]
    public void Allows_interleaved_calls_between_steps()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);

        mock.Instance.GetTemplate("welcome", 1);
        mock.Instance.GetTemplate("footer", 2);  // interleaved - should be ignored
        mock.Instance.Send("a@b.com", "hi");

        mock.VerifyInOrder(
            mock.GetTemplate(Any, Any),
            mock.Send(Any, Any)
        );
    }

    [Fact]
    public void Matchers_are_respected_per_step()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Returns(true);

        mock.Instance.Send("a@b.com", "hi");
        mock.Instance.Send("b@b.com", "hi");

        // First Send to a@b.com, then any Send - order + matchers both apply
        mock.VerifyInOrder(
            mock.Send("a@b.com", Any),
            mock.Send("b@b.com", Any)
        );
    }

    [Fact]
    public void Fails_with_message_naming_the_missing_step()
    {
        var mock = new MockEmailService();

        mock.Instance.GetTemplate("welcome", 1);
        // Send never called

        var ex = Assert.Throws<VerificationException>(() =>
            mock.VerifyInOrder(
                mock.GetTemplate(Any, Any),
                mock.Send(Any, Any)
            ));

        Assert.Contains("Send", ex.Message);
        Assert.Contains("GetTemplate", ex.Message);
    }

    [Fact]
    public void Works_with_void_and_non_void_methods_mixed()
    {
        var mock = new MockUserRepository();
        mock.GetUserAsync(Any).Returns((int id) => Task.FromResult($"User#{id}"));

        mock.Instance.GetUserAsync(1);
        mock.Instance.Delete(1);

        mock.VerifyInOrder(
            mock.GetUserAsync(Any),
            mock.Delete(Any)
        );
    }
}
