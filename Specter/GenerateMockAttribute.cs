using System;

namespace Specter;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class GenerateMockAttribute : Attribute
{
    public Type TargetType { get; }
    public GenerateMockAttribute(Type targetType) => TargetType = targetType;
}