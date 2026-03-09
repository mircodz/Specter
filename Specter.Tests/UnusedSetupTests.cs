using System.Collections.Generic;
using Xunit;
using static Specter.Wildcard;

namespace Specter.Tests;

public class UnusedSetupTests
{
    [Fact]
    public void Calls_callback_when_setup_never_matched()
    {
        var warnings = new List<string>();
        var mock = new MockEmailService(onUnusedSetup: warnings.Add);
        mock.Setup(x => x.Send(Any, Any)).Returns(true);

        mock.CheckUnusedSetups();

        Assert.Single(warnings);
        Assert.Contains("Send", warnings[0]);
    }

    [Fact]
    public void No_callback_when_all_setups_matched()
    {
        var warnings = new List<string>();
        var mock = new MockEmailService(onUnusedSetup: warnings.Add);
        mock.Setup(x => x.Send(Any, Any)).Returns(true);

        mock.Instance.Send("a@b.com", "hi");
        mock.CheckUnusedSetups();

        Assert.Empty(warnings);
    }

    [Fact]
    public void Single_callback_lists_all_unused_setups()
    {
        var warnings = new List<string>();
        var mock = new MockEmailService(onUnusedSetup: warnings.Add);
        mock.Setup(x => x.Send(Any, Any)).Returns(true);
        mock.Setup(x => x.GetTemplate(Any, Any)).Returns("hi");

        mock.CheckUnusedSetups();

        Assert.Single(warnings); // one message, two entries
        Assert.Contains("Send", warnings[0]);
        Assert.Contains("GetTemplate", warnings[0]);
    }

    [Fact]
    public void Only_unmatched_setups_reported()
    {
        var warnings = new List<string>();
        var mock = new MockEmailService(onUnusedSetup: warnings.Add);
        mock.Setup(x => x.Send(Any, Any)).Returns(true);
        mock.Setup(x => x.GetTemplate(Any, Any)).Returns("hi");

        mock.Instance.Send("a@b.com", "hi");
        mock.CheckUnusedSetups();

        Assert.Single(warnings);
        Assert.DoesNotContain("Send", warnings[0]);
        Assert.Contains("GetTemplate", warnings[0]);
    }

    [Fact]
    public void Disabled_when_no_callback_provided()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Any, Any)).Returns(true);

        // Should not throw — no callback = no check
        mock.CheckUnusedSetups();
    }
}
