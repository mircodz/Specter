using System;
using System.Collections.Generic;

namespace Specter;

public interface IMatcher
{
    bool Matches(object? actual);
    string Describe();
    void OnMatched(object? actual) { }
}

public class AnyMatcher : IMatcher
{
    public bool Matches(object? _) => true;
    public string Describe() => "_";
}

public class EqualityMatcher<T>(T expected) : IMatcher
{
    public bool Matches(object? actual)
    {
        if (actual is T t)
        {
            return EqualityComparer<T>.Default.Equals(t, expected);
        }

        if (actual is null && expected is null)
        {
            return true;
        }

        return false;
    }

    public string Describe() => $"{expected}";
}

public class PredicateMatcher<T>(Func<T, bool> pred, string label = "predicate") : IMatcher
{
    public bool Matches(object? actual) => actual is T t && pred(t);
    public string Describe() => $"Is({label})";
}

public class Matcher<T>
{
    public IMatcher Inner { get; }
    private Matcher(IMatcher inner) => Inner = inner;

    public static readonly Matcher<T> Any = new(new AnyMatcher());

    public static Matcher<T> From(IMatcher inner) => new(inner);

    public static Matcher<T> Is(Func<T, bool> pred, string label = "predicate")
        => new(new PredicateMatcher<T>(pred, label));

    // Wildcard _ → Matcher<T>.Any for any T — makes _ universally usable
    public static implicit operator Matcher<T>(Wildcard _) => Any;

    // Concrete value → equality matcher (enables passing "alice@test.com" directly)
    public static implicit operator Matcher<T>(T value)
        => new(new EqualityMatcher<T>(value));

    public bool Matches(object? value) => Inner.Matches(value);
    public override string ToString() => Inner.Describe();
}
