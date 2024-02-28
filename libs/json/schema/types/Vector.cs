using System;
using System.Linq;
using Godot;
using GodotDictionary = Godot.Collections.Dictionary;

namespace JSON.Schema.Type;

internal class Vector : JSON
{
    private readonly Variant.Type Type;
    
    public Vector(Variant.Type type, IJSONType elementSchema, Variant? defaultValue = null, params StringName[] elementNames)
    {
        Type = type;
        Properties = elementNames.Select(name => (name, elementSchema)).ToDictionary();
        RequiredProperties = [..elementNames.Where(el => el != "A")];
        Default = defaultValue?.JSONSerialize();
        AdditionalProperties = false;
    }
    public static Vector Vector2(Variant? defaultValue = null) => new(Variant.Type.Vector2, new Number<float>(), defaultValue, "X", "Y");
    public static Vector Vector2I(Variant? defaultValue = null) => new(Variant.Type.Vector2I, new Number<int>(), defaultValue, "X", "Y");
    public static Vector Vector3(Variant? defaultValue = null) => new(Variant.Type.Vector3, new Number<float>(), defaultValue, "X", "Y", "Z");
    public static Vector Vector3I(Variant? defaultValue = null) => new(Variant.Type.Vector3I, new Number<int>(), defaultValue, "X", "Y", "Z");
    public static Vector Vector4(Variant? defaultValue = null) => new(Variant.Type.Vector4, new Number<float>(), defaultValue, "X", "Y", "Z", "W");
    public static Vector Vector4I(Variant? defaultValue = null) => new(Variant.Type.Vector4I, new Number<int>(), defaultValue, "X", "Y", "Z", "W");
    public static Vector ColorNoAlpha(Variant? defaultValue = null) => new(Variant.Type.Color, new Number<float> { Min = 0.0f, Max = 1.0f }, defaultValue, "R", "G", "B");
    public static Vector Color(Variant? defaultValue = null) => new(Variant.Type.Color, new Number<float> { Min = 0.0f, Max = 1.0f }, defaultValue, "R", "G", "B", "A");
    public new static Vector FromProperty(Property property) => property.Ty switch
    {
        Variant.Type.Vector2 => Vector2(property.Default),
        Variant.Type.Vector2I => Vector2I(property.Default),
        Variant.Type.Vector3 => Vector3(property.Default),
        Variant.Type.Vector3I => Vector3I(property.Default),
        Variant.Type.Vector4 => Vector4(property.Default),
        Variant.Type.Vector4I => Vector4I(property.Default),
        Variant.Type.Color when property.Hint == PropertyHint.ColorNoAlpha => ColorNoAlpha(property.Default),
        Variant.Type.Color => Color(property.Default),
        _ => throw new NotImplementedException($"Not implemented: Vector.FromProperty({property.Name})")
    };
    
    public new GodotDictionary ToDict() => this.WithDefault<Vector, GodotDictionary>(GetSchema());
    
    public new Variant? Clean(Variant data)
    {
        var cleaned = base.Clean(data);
        if (cleaned.Obj is GodotDictionary dict)
            return Type switch
            {
                Variant.Type.Vector2 => new Vector2(dict["X"].As<float>(), dict["Y"].As<float>()),
                Variant.Type.Vector2I => new Vector2I(dict["X"].As<int>(), dict["Y"].As<int>()),
                Variant.Type.Vector3 => new Vector3(dict["X"].As<float>(), dict["Y"].As<float>(), dict["Z"].As<float>()),
                Variant.Type.Vector3I => new Vector3I(dict["X"].As<int>(), dict["Y"].As<int>(), dict["Z"].As<int>()),
                Variant.Type.Vector4 => new Vector4(dict["X"].As<float>(), dict["Y"].As<float>(), dict["Z"].As<float>(), dict["W"].As<float>()),
                Variant.Type.Vector4I => new Vector4I(dict["X"].As<int>(), dict["Y"].As<int>(), dict["Z"].As<int>(), dict["W"].As<int>()),
                Variant.Type.Color => new Color(dict["R"].As<float>(), dict["G"].As<float>(), dict["B"].As<float>(), dict.Get("A", 1.0f)),
                _ => throw new NotImplementedException($"Not implemented: Vector.Clean({dict})")
            };
        return null;
    }
}