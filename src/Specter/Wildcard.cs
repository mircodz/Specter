namespace Specter;

public sealed class Wildcard
{
    private Wildcard() { }
    public static readonly Wildcard _ = new();
}