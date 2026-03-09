using System;
using Xunit;
using static Specter.Wildcard;

namespace Specter.Tests;

public class VoidMethodTests
{
    [Fact]
    public void Callback_is_invoked()
    {
        var mock = new MockUserRepository();
        var invoked = false;

        mock.Setup(x => x.Delete(Any)).Callback(() => invoked = true);

        mock.Instance.Delete(42);
        Assert.True(invoked);
    }

    [Fact]
    public void Throws_on_matched_call()
    {
        var mock = new MockUserRepository();
        mock.Setup(x => x.Delete(99)).Throws(new InvalidOperationException("Cannot delete"));

        Assert.Throws<InvalidOperationException>(() => mock.Instance.Delete(99));
    }

    [Fact]
    public void Callback_captures_arguments()
    {
        var mock = new MockUserRepository();
        object?[]? captured = null;

        mock.Setup(x => x.Delete(Any)).Callback(args => captured = args);

        mock.Instance.Delete(7);
        Assert.NotNull(captured);
        Assert.Equal(7, captured![0]);
    }
}