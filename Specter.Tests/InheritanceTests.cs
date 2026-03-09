using Xunit;

namespace Specter.Tests;

public class InheritanceTests
{
    [Fact]
    public void Mock_implements_base_and_derived_methods()
    {
        var mock = new MockExtendedService();
        mock.Setup(x => x.GetName()).Returns("TestService");
        mock.Setup(x => x.GetCount()).Returns(42);

        Assert.Equal("TestService", mock.Instance.GetName());
        Assert.Equal(42, mock.Instance.GetCount());
    }

    [Fact]
    public void Mock_can_be_used_as_base_interface()
    {
        var mock = new MockExtendedService();
        mock.Setup(x => x.GetName()).Returns("Base");

        IBaseService baseService = mock.Instance;
        Assert.Equal("Base", baseService.GetName());
    }

    [Fact]
    public void Instance_returns_interface_implementation()
    {
        var mock = new MockEmailService();
        IEmailService service = mock.Instance;

        Assert.NotNull(service);
        Assert.IsType<MockEmailService>(service);
    }
}