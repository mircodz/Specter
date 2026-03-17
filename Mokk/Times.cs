namespace Mokk;

public readonly struct Times
{
    private readonly int _min;
    private readonly int _max;
    private readonly string _description;

    private Times(int min, int max, string description)
    {
        _min = min;
        _max = max;
        _description = description;
    }

    public static readonly Times Once = new(1, 1, "exactly once");
    public static readonly Times Never = new(0, 0, "never");
    public static readonly Times AtLeastOnce = new(1, int.MaxValue, "at least once");
    public static readonly Times AtMostOnce = new(0, 1, "at most once");

    public static Times Exactly(int n) => new(n, n, $"exactly {n}");
    public static Times AtLeast(int n) => new(n, int.MaxValue, $"at least {n}");
    public static Times AtMost(int n) => new(0, n, $"at most {n}");
    public static Times Between(int min, int max) => new(min, max, $"between {min} and {max}");

    public bool IsMatch(int count) => count >= _min && count <= _max;
    public string Describe(int actual) => $"expected {_description} calls, got {actual}";
}