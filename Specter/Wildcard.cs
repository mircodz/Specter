namespace Specter;

public sealed class Wildcard
{
    private Wildcard() { }

    /// <summary>Wildcard matcher — matches any argument. Use via <c>using static Specter.Wildcard</c>.</summary>
    public static readonly Wildcard Any = new();
}
