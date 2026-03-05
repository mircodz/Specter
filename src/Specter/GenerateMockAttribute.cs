namespace Specter;

[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
public class GenerateMockAttribute : Attribute
{
    public Type InterfaceType { get; }
    public GenerateMockAttribute(Type interfaceType) => InterfaceType = interfaceType;
}