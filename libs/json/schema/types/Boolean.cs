using Godot;

using GodotDictionary = Godot.Collections.Dictionary;

namespace JSON.Schema.Type;

public class Boolean : IJSONType
{
    public Variant? Default { get; init; }
    public static Boolean FromProperty(Property property) => new() { Default = property.Default };
    public GodotDictionary GetSchema() => this.WithDefault<Boolean, bool>(new GodotDictionary { { "type", "boolean" } });

    public bool IsValid(Variant data) => data.VariantType == Variant.Type.Bool;
    public Variant Clean(Variant data) => IsValid(data) ? data : throw new InvalidDataException(this, data);
}