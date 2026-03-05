using Specter;
using Xunit;
using static Specter.Wildcard;

namespace Specter.Tests;

public class WildcardMatchingTests
{
    [Fact]
    public void Wildcard_matches_any_argument()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(_, _)).Returns(true);

        Assert.True(mock.Instance.Send("anyone@test.com", "Hello"));
        Assert.True(mock.Instance.Send("other@test.com", "World"));
    }

    [Fact]
    public void Exact_value_match_via_implicit_conversion()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send("admin@site.com", _)).Returns(true);

        Assert.True(mock.Instance.Send("admin@site.com", "anything"));
        Assert.False(mock.Instance.Send("other@site.com", "anything"));
    }

    [Fact]
    public void Last_setup_wins_over_earlier_wildcard()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(_, _)).Returns(true);
        mock.Setup(x => x.Send("blocked@evil.com", _)).Returns(false);

        Assert.False(mock.Instance.Send("blocked@evil.com", "anything"));
        Assert.True(mock.Instance.Send("legit@good.com", "anything"));
    }

    [Fact]
    public void Mixed_types_int_version_with_wildcard_string()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.GetTemplate(_, 2)).Returns("v2-template");

        Assert.Equal("v2-template", mock.Instance.GetTemplate("welcome", 2));
        Assert.Equal("v2-template", mock.Instance.GetTemplate("any-name", 2));
        Assert.Null(mock.Instance.GetTemplate("welcome", 3));
    }
}

public class PredicateMatcherTests
{
    [Fact]
    public void Predicate_matcher_with_Matcher_Is()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(
            Matcher<string>.Is(s => s.EndsWith("@internal.com"), "@internal.com"),
            _
        )).Returns(true);

        Assert.True(mock.Instance.Send("alice@internal.com", "hi"));
        Assert.False(mock.Instance.Send("alice@external.com", "hi"));
    }

    [Fact]
    public void Predicate_matcher_with_Arg_Is()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(
            Arg.Is<string>(s => s.Contains("@")),
            _
        )).Returns(true);

        Assert.True(mock.Instance.Send("test@example.com", "subject"));
        Assert.False(mock.Instance.Send("no-at-sign", "subject"));
    }

    [Fact]
    public void Arg_Any_matches_any_value()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(Arg.Any<string>(), Arg.Any<string>())).Returns(true);

        Assert.True(mock.Instance.Send("any@any.com", "any"));
    }
}

public class ExceptionTests
{
    [Fact]
    public void Throws_on_matched_call()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send("bad@evil.com", _))
            .Throws(new InvalidOperationException("Blocked!"));

        var ex = Assert.Throws<InvalidOperationException>(() =>
            mock.Instance.Send("bad@evil.com", "test"));
        Assert.Equal("Blocked!", ex.Message);
    }

    [Fact]
    public void Throws_generic_exception_type()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(_, _)).Throws<ArgumentException>();

        Assert.Throws<ArgumentException>(() => mock.Instance.Send("a", "b"));
    }
}

public class VoidMethodTests
{
    [Fact]
    public void Callback_is_invoked()
    {
        var mock = new MockUserRepository();
        var invoked = false;

        mock.Setup(x => x.Delete(_)).Callback(() => invoked = true);

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

        mock.Setup(x => x.Delete(_)).Callback(args => captured = args);

        mock.Instance.Delete(7);
        Assert.NotNull(captured);
        Assert.Equal(7, captured![0]);
    }
}

public class PropertyTests
{
    [Fact]
    public void Getter_returns_setup_value()
    {
        var mock = new MockUserRepository();
        mock.Setup(x => x.Name).Returns("Alice");
        mock.Setup(x => x.Age).Returns(30);

        Assert.Equal("Alice", mock.Instance.Name);
        Assert.Equal(30, mock.Instance.Age);
    }

    [Fact]
    public void Setter_is_intercepted_and_verifiable()
    {
        var mock = new MockUserRepository();
        mock.Instance.Name = "Bob";

        mock.Verify(x => x.SetName(_), exactly: 1);
    }
}

public class AsyncMethodTests
{
    [Fact]
    public async Task ReturnsAsync_for_Task()
    {
        var mock = new MockUserRepository();
        mock.Setup(x => x.GetUserAsync(42)).ReturnsAsync("Alice");

        Assert.Equal("Alice", await mock.Instance.GetUserAsync(42));
    }

    [Fact]
    public async Task ReturnsAsync_for_ValueTask()
    {
        var mock = new MockUserRepository();
        mock.Setup(x => x.CountAsync()).ReturnsAsync(5);

        Assert.Equal(5, await mock.Instance.CountAsync());
    }
}

public class SequenceTests
{
    [Fact]
    public void Returns_values_in_order()
    {
        var mock = new MockEmailService();
        mock.SetupSequence(x => x.Send(_, _))
            .Returns(true)
            .Returns(false)
            .Returns(true);

        Assert.True(mock.Instance.Send("a", "b"));
        Assert.False(mock.Instance.Send("c", "d"));
        Assert.True(mock.Instance.Send("e", "f"));
    }

    [Fact]
    public void Returns_default_after_exhausted()
    {
        var mock = new MockEmailService();
        mock.SetupSequence(x => x.GetTemplate(_, _))
            .Returns("first")
            .Returns("second");

        Assert.Equal("first", mock.Instance.GetTemplate("a", 1));
        Assert.Equal("second", mock.Instance.GetTemplate("b", 2));
        Assert.Null(mock.Instance.GetTemplate("c", 3));
    }

    [Fact]
    public void Throws_in_sequence()
    {
        var mock = new MockEmailService();
        mock.SetupSequence(x => x.Send(_, _))
            .Returns(true)
            .Throws<InvalidOperationException>();

        Assert.True(mock.Instance.Send("a", "b"));
        Assert.Throws<InvalidOperationException>(() => mock.Instance.Send("c", "d"));
    }
}

public class CallbackTests
{
    [Fact]
    public void Executes_on_each_matched_call()
    {
        var mock = new MockEmailService();
        var count = 0;

        mock.Setup(x => x.Send(_, _))
            .Callback(() => count++)
            .Returns(true);

        mock.Instance.Send("a", "b");
        mock.Instance.Send("c", "d");
        Assert.Equal(2, count);
    }

    [Fact]
    public void Captures_arguments()
    {
        var mock = new MockEmailService();
        var capturedTo = "";

        mock.Setup(x => x.Send(_, _))
            .Callback(args => capturedTo = (string)args[0]!)
            .Returns(true);

        mock.Instance.Send("target@test.com", "subject");
        Assert.Equal("target@test.com", capturedTo);
    }
}

public class VerificationTests
{
    [Fact]
    public void Exactly_N_calls()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(_, _)).Returns(true);

        mock.Instance.Send("a@test.com", "s1");
        mock.Instance.Send("b@test.com", "s2");

        mock.Verify(x => x.Send(_, _), exactly: 2);
    }

    [Fact]
    public void AtLeast_N_calls()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(_, _)).Returns(true);

        mock.Instance.Send("a@test.com", "s1");
        mock.Instance.Send("b@test.com", "s2");
        mock.Instance.Send("c@test.com", "s3");

        mock.Verify(x => x.Send(_, _), atLeast: 2);
    }

    [Fact]
    public void Fails_when_count_doesnt_match()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(_, _)).Returns(true);
        mock.Instance.Send("a@test.com", "s1");

        Assert.Throws<VerificationException>(() =>
            mock.Verify(x => x.Send(_, _), exactly: 2));
    }

    [Fact]
    public void Scoped_to_specific_argument()
    {
        var mock = new MockEmailService();
        mock.Setup(x => x.Send(_, _)).Returns(true);

        mock.Instance.Send("admin@site.com", "s1");
        mock.Instance.Send("admin@site.com", "s2");
        mock.Instance.Send("other@site.com", "s3");

        mock.Verify(x => x.Send("admin@site.com", _), exactly: 2);
    }

    [Fact]
    public void Void_method_calls()
    {
        var mock = new MockUserRepository();
        mock.Instance.Delete(1);
        mock.Instance.Delete(2);

        mock.Verify(x => x.Delete(_), exactly: 2);
    }
}

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
