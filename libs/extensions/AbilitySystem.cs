using AS;

public static class AttributeExtensions
{
    public static bool IsAttributeLow(this AbilitySystem system, Attribute attribute)
    {
        if (!system.HasAttribute(attribute)) throw new System.Exception("Missing attribute!");
        var threshold = ((attribute.MaxValue - attribute.MinValue) / 2) + attribute.MinValue;
        return system.GetAttributeValue(attribute) < threshold;
    }
}