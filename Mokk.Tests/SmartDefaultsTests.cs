using System.Threading.Tasks;
using Xunit;

namespace Mokk.Tests;

public class SmartDefaultsTests
{
    [Fact]
    public void String_returns_empty_string()
    {
        var mock = new MockEmailService();
        Assert.Equal("", mock.Instance.GetTemplate("x", 1));
    }

    [Fact]
    public void Bool_returns_false()
    {
        var mock = new MockEmailService();
        Assert.False(mock.Instance.Send("a@b.com", "hi"));
    }

    [Fact]
    public async Task Task_of_T_returns_completed_task_with_default()
    {
        var mock = new MockUserRepository();
        // Task<string> completes without throwing - inner value is default(string) = null
        var task = mock.Instance.GetUserAsync(1);
        Assert.NotNull(task);
        var result = await task;
        Assert.Null(result);
    }

    [Fact]
    public async Task ValueTask_of_T_returns_default()
    {
        var mock = new MockUserRepository();
        var result = await mock.Instance.CountAsync();
        Assert.Equal(0, result);
    }

    [Fact]
    public async Task Task_returns_completed_task()
    {
        // Task (non-generic) should not throw on await
        var mock = new MockUserRepository();
        var task = mock.Instance.GetUserAsync(1);
        Assert.NotNull(task);
        await task; // should complete without exception
    }
}