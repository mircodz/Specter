using Xunit;
using static Mokk.Wildcard;

namespace Mokk.Tests;

public class CallbackTests
{
    [Fact]
    public void Executes_on_each_matched_call()
    {
        var mock = new MockEmailService();
        var count = 0;

        mock.Send(Any, Any)
            .Callback(() => count++)
            .Returns(true);

        mock.Instance.Send("a", "b");
        mock.Instance.Send("c", "d");
        Assert.Equal(2, count);
    }

    [Fact]
    public void Captures_arguments()
    {
        var mock = new MockEmailService();
        var capturedTo = "";

        mock.Send(Any, Any)
            .Callback(args => capturedTo = (string)args[0]!)
            .Returns(true);

        mock.Instance.Send("target@test.com", "subject");
        Assert.Equal("target@test.com", capturedTo);
    }
}
