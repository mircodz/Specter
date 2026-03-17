using System.Threading.Tasks;
using Xunit;
using static Specter.Wildcard;

namespace Specter.Tests;

public class TypedReturnsFactoryTests
{
    [Fact]
    public void Returns_with_typed_args()
    {
        var mock = new MockEmailService();
        mock.GetTemplate(Any, Any).Returns((string name, int version) => $"{name}-v{version}");

        Assert.Equal("welcome-v2", mock.Instance.GetTemplate("welcome", 2));
        Assert.Equal("reset-v1", mock.Instance.GetTemplate("reset", 1));
    }

    [Fact]
    public void Returns_factory_single_arg()
    {
        var mock = new MockUserRepository();
        mock.GetUserAsync(Any).Returns((int id) => Task.FromResult($"User#{id}"));

        Assert.Equal("User#42", mock.Instance.GetUserAsync(42).Result);
        Assert.Equal("User#99", mock.Instance.GetUserAsync(99).Result);
    }
}
