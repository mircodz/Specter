using System;
using Xunit;
using static Specter.Wildcard;

namespace Specter.Tests;

public class ExceptionTests
{
    [Fact]
    public void Throws_on_matched_call()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send("bad@evil.com", Any))
            .Throws(new InvalidOperationException("Blocked!"));

        var ex = Assert.Throws<InvalidOperationException>(() =>
            mock.Instance.Send("bad@evil.com", "test"));
        Assert.Equal("Blocked!", ex.Message);
    }

    [Fact]
    public void Throws_generic_exception_type()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Any, Any)).Throws<ArgumentException>();

        Assert.Throws<ArgumentException>(() => mock.Instance.Send("a", "b"));
    }
}