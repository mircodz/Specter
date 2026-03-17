using Xunit;
using static Specter.Wildcard;

namespace Specter.Tests;

public class GenericMethodTests
{
    [Fact]
    public void Exact_type_setup_returns_value()
    {
        var mock = new MockTemplatedService();
        mock.DoSomething<int>(Any).Returns(1);

        Assert.Equal(1, mock.Instance.DoSomething(123));
    }

    [Fact]
    public void Different_type_args_are_independent()
    {
        var mock = new MockTemplatedService();
        mock.DoSomething<int>(Any).Returns(1);
        mock.DoSomething<string>(Any).Returns("hello");

        Assert.Equal(1, mock.Instance.DoSomething<int>(0));
        Assert.Equal("hello", mock.Instance.DoSomething<string>(""));
    }

    [Fact]
    public void AnyType_wildcard_matches_all_type_args_in_verify()
    {
        var mock = new MockTemplatedService();

        mock.Instance.DoSomething<int>(1);
        mock.Instance.DoSomething<string>("x");

        mock.DoSomething<AnyType>(Any).Verify(Times.Exactly(2));
    }

    [Fact]
    public void AnyType_wildcard_matches_for_callback()
    {
        var mock = new MockTemplatedService();
        var count = 0;
        mock.DoSomething<AnyType>(Any).Callback(() => count++);

        mock.Instance.DoSomething<int>(1);
        mock.Instance.DoSomething<string>("x");

        Assert.Equal(2, count);
    }

    [Fact]
    public void Exact_type_wins_over_AnyType_wildcard()
    {
        var mock = new MockTemplatedService();
        mock.DoSomething<AnyType>(Any).Callback(() => { });
        mock.DoSomething<int>(Any).Returns(99);

        Assert.Equal(99, mock.Instance.DoSomething<int>(0));
    }
}
