using System;

namespace Specter;

public static class Arg
{
    public static Matcher<T> Any<T>() => Matcher<T>.Any;

    public static Matcher<T> Is<T>(Func<T, bool> predicate)
        => Matcher<T>.Is(predicate);

    public static Matcher<T> Is<T>(Func<T, bool> predicate, string label)
        => Matcher<T>.Is(predicate, label);
}