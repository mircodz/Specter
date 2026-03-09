using Xunit;

namespace Specter.Tests;

public class AutoPropertyBackingStoreTests
{
    [Fact]
    public void Set_then_get_returns_set_value()
    {
        var mock = new MockUserRepository();
        mock.Instance.Name = "Alice";
        Assert.Equal("Alice", mock.Instance.Name);
    }

    [Fact]
    public void Get_without_set_falls_through_to_interceptor()
    {
        var mock = new MockUserRepository();
        mock.Setup(x => x.Name).Returns("FromSetup");
        Assert.Equal("FromSetup", mock.Instance.Name);
    }

    [Fact]
    public void Set_value_takes_priority_over_setup()
    {
        var mock = new MockUserRepository();
        mock.Setup(x => x.Name).Returns("FromSetup");
        mock.Instance.Name = "Direct";
        Assert.Equal("Direct", mock.Instance.Name);
    }

    [Fact]
    public void Reset_clears_backing_store()
    {
        var mock = new MockUserRepository();
        mock.Instance.Name = "Alice";
        mock.Reset();
        // After reset, no setup and no backing — falls through to smart default
        Assert.Equal("", mock.Instance.Name);
    }

    [Fact]
    public void Read_only_property_still_works_via_interceptor()
    {
        var mock = new MockUserRepository();
        mock.Setup(x => x.Age).Returns(30);
        Assert.Equal(30, mock.Instance.Age);
    }
}