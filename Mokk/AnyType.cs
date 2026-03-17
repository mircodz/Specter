namespace Mokk;

/// <summary>
/// Sentinel type used in generic type-argument matching. When <see cref="AnyType"/> appears in
/// a type-args array, it matches any concrete type argument - equivalent to a wildcard for generics.
/// </summary>
public sealed class AnyType
{
    private AnyType() { }
}
