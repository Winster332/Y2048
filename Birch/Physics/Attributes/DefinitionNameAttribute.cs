namespace Birch.Physics.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class DefinitionNameAttribute : Attribute
{
    public string Name { get; }
    public DefinitionNameAttribute(string name)
    {
        Name = name;
    }
}
