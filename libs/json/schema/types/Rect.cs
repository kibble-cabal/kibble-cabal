using System;
using System.Linq;
using Godot;

using GodotDictionary = Godot.Collections.Dictionary;

namespace JSON.Schema.Type;

internal class Rect : JSON
{
    public Rect(IJSONType elementSchema, Variant? defaultValue = null)
    {
        StringName[] propertyNames = ["Size", "Position"];
        Properties = propertyNames.Select(name => (name, elementSchema)).ToDictionary();
        RequiredProperties = propertyNames;
        Default = defaultValue?.JSONSerialize();
        AdditionalProperties = false;
    }
    public new static Rect FromProperty(Property property) => property.Ty switch
    {
        Variant.Type.Rect2 => new Rect(Vector.Vector2()),
        Variant.Type.Rect2I => new Rect(Vector.Vector2I()),
        _ => throw new NotImplementedException($"Not implemented: Rect.FromProperty({property.Name})")
    };
    public new GodotDictionary ToDict() => this.WithDefault<Rect, GodotDictionary>(base.GetSchema());
}