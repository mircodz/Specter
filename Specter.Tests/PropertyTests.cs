using Xunit;
using static Specter.Wildcard;

namespace Specter.Tests;

public class PropertyTests
{
    [Fact]
    public void Getter_returns_setup_value()
    {
        var mock = new MockUserRepository();
        mock.Name.Getter().Returns("Alice");
        mock.Age.Getter().Returns(30);

        Assert.Equal("Alice", mock.Instance.Name);
        Assert.Equal(30, mock.Instance.Age);
    }

    [Fact]
    public void Setter_is_intercepted_and_verifiable()
    {
        var mock = new MockUserRepository();
        mock.Instance.Name = "Bob";

        mock.Name.Setter(Any).Verify(Times.Once);
    }
}
