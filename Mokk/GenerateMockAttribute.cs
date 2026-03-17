using System;

namespace Mokk;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class GenerateMockAttribute : Attribute
{
    public Type TargetType { get; }
    public GenerateMockAttribute(Type targetType) => TargetType = targetType;
}