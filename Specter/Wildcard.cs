namespace Specter;

/// <summary>
/// Wildcard sentinel used as a match-any argument. Implicitly converts to <see cref="Matcher{T}"/>
/// for any T, enabling both <c>_</c> (via <c>using static Specter.Wildcard</c>) and <c>Any</c> usage.
/// </summary>
public sealed class Wildcard
{
    private Wildcard() { }

    /// <summary>Match-any shorthand. Use via <c>using static Specter.Wildcard</c>.</summary>
    public static readonly Wildcard _ = new();

    /// <summary>Match-any. Use directly as <c>Wildcard.Any</c> or import with <c>using static</c>.</summary>
    public static readonly Wildcard Any = new();
}
