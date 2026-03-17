namespace Mokk;

public class Slot<T>
{
    public T? Value { get; internal set; }
    public bool HasValue { get; internal set; }
}

public class CaptureMatcher<T>(Slot<T> slot) : IMatcher
{
    public bool Matches(object? actual) => true;
    public void OnMatched(object? actual) { slot.Value = (T?)actual; slot.HasValue = true; }
    public string Describe() => $"capture<{typeof(T).Name}>";
}

public static class Capture
{
    public static Slot<T> Slot<T>() => new();
    public static Matcher<T> Into<T>(Slot<T> slot) => Matcher<T>.From(new CaptureMatcher<T>(slot));
}
