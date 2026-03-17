using Xunit;
using static Specter.Wildcard;
using static Specter.Capture;

namespace Specter.Tests;

public class CaptureSlotTests
{
    [Fact]
    public void Captures_argument_on_match()
    {
        var slot = Slot<string>();
        var mock = new MockEmailService();
        mock.Send(Into(slot), Any).Returns(true);

        mock.Instance.Send("hello@test.com", "subject");

        Assert.True(slot.HasValue);
        Assert.Equal("hello@test.com", slot.Value);
    }

    [Fact]
    public void Slot_updated_on_each_call()
    {
        var slot = Slot<string>();
        var mock = new MockEmailService();
        mock.Send(Into(slot), Any).Returns(true);

        mock.Instance.Send("first@test.com", "subject");
        mock.Instance.Send("second@test.com", "subject");

        Assert.Equal("second@test.com", slot.Value);
    }

    [Fact]
    public void HasValue_false_when_never_called()
    {
        var slot = Slot<string>();
        var mock = new MockEmailService();
        mock.Send(Into(slot), Any).Returns(true);

        Assert.False(slot.HasValue);
        Assert.Null(slot.Value);
    }

    [Fact]
    public void Capture_does_not_fire_for_non_matching_setup()
    {
        var slot = Slot<string>();
        var mock = new MockEmailService();
        // First setup captures but only matches specific subject
        mock.Send(Into(slot), "specific").Returns(true);
        // Second setup is the one that actually wins
        mock.Send(Any, Any).Returns(false);

        mock.Instance.Send("hello@test.com", "other");

        // Slot should not have been captured - the capturing setup didn't win
        Assert.False(slot.HasValue);
    }

    [Fact]
    public void Can_capture_int_argument()
    {
        var slot = Slot<int>();
        var mock = new MockEmailService();
        mock.GetTemplate(Any, Into(slot)).Returns("result");

        mock.Instance.GetTemplate("welcome", 42);

        Assert.Equal(42, slot.Value);
    }

    [Fact]
    public void Multiple_slots_in_same_setup()
    {
        var toSlot = Slot<string>();
        var subjectSlot = Slot<string>();
        var mock = new MockEmailService();
        mock.Send(Into(toSlot), Into(subjectSlot)).Returns(true);

        mock.Instance.Send("a@test.com", "hello");

        Assert.Equal("a@test.com", toSlot.Value);
        Assert.Equal("hello", subjectSlot.Value);
    }
}
