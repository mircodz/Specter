using System;
using Xunit;
using static Mokk.Wildcard;

namespace Mokk.Tests;

public class SequenceTests
{
    [Fact]
    public void Returns_values_in_order()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Sequence()
            .Returns(true)
            .Returns(false)
            .Returns(true);

        Assert.True(mock.Instance.Send("a", "b"));
        Assert.False(mock.Instance.Send("c", "d"));
        Assert.True(mock.Instance.Send("e", "f"));
    }

    [Fact]
    public void Returns_default_after_exhausted()
    {
        var mock = new MockEmailService();
        mock.GetTemplate(Any, Any).Sequence()
            .Returns("first")
            .Returns("second");

        Assert.Equal("first", mock.Instance.GetTemplate("a", 1));
        Assert.Equal("second", mock.Instance.GetTemplate("b", 2));
        Assert.Equal("", mock.Instance.GetTemplate("c", 3));
    }

    [Fact]
    public void Throws_in_sequence()
    {
        var mock = new MockEmailService();
        mock.Send(Any, Any).Sequence()
            .Returns(true)
            .Throws<InvalidOperationException>();

        Assert.True(mock.Instance.Send("a", "b"));
        Assert.Throws<InvalidOperationException>(() => mock.Instance.Send("c", "d"));
    }
}
