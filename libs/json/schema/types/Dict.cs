using Godot;

using GodotDictionary = Godot.Collections.Dictionary;

namespace JSON.Schema.Type;

internal class Dict : IJSONType
{
    public Variant? Default { get; set; }
    public static Dict FromProperty(Property property) => new() { Default = property.Default };
    public GodotDictionary GetSchema() => this.WithDefault<Dict, GodotDictionary>(new GodotDictionary { { "type", "object" }, { "additionalProperties", true } });

    public bool IsValid(Variant data) => data.VariantType == Variant.Type.Dictionary;
    public Variant Clean(Variant data) => IsValid(data) ? data : throw new InvalidDataException(this, data);
}