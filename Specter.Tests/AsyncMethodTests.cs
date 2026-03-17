using System.Threading.Tasks;
using Xunit;
using static Specter.Wildcard;

namespace Specter.Tests;

public class AsyncMethodTests
{
    [Fact]
    public async Task ReturnsAsync_for_Task()
    {
        var mock = new MockUserRepository();
        mock.GetUserAsync(42).ReturnsAsync("Alice");

        Assert.Equal("Alice", await mock.Instance.GetUserAsync(42));
    }

    [Fact]
    public async Task ReturnsAsync_for_ValueTask()
    {
        var mock = new MockUserRepository();
        mock.CountAsync().ReturnsAsync(5);

        Assert.Equal(5, await mock.Instance.CountAsync());
    }
}
