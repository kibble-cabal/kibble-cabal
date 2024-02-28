using System;
using System.Collections;
using System.Linq;
using Godot;

using GodotArray = Godot.Collections.Array;
using GodotDictionary = Godot.Collections.Dictionary;

namespace JSON.Schema.Type;

internal class Array(IJSONType? elementSchema = null, Variant? defaultValue = null) : IJSONType
{
    public readonly IJSONType? ElementSchema = elementSchema;
    public Variant? Default { get; } = defaultValue;

    public static Array FromProperty(Property property)
    {
        if (property.Ty != Variant.Type.Array) return property.Ty switch
        {
            Variant.Type.PackedByteArray
                or Variant.Type.PackedInt32Array
                or Variant.Type.PackedInt64Array => new Array(new Number<int>(), property.Default),
            Variant.Type.PackedFloat32Array
                or Variant.Type.PackedFloat64Array => new Array(new Number<float>(), property.Default),
            Variant.Type.PackedStringArray => new Array(new String(), property.Default),
            Variant.Type.PackedVector2Array => new Array(Vector.Vector2()),
            Variant.Type.PackedVector3Array => new Array(Vector.Vector3()),
            Variant.Type.PackedColorArray => new Array(Vector.Color()),
            _ => throw new NotImplementedException($"Not implemented: Array.FromProperty({property.Name})")
        };
        var strings = property.HintString.Split(["/", ":"], StringSplitOptions.TrimEntries & StringSplitOptions.RemoveEmptyEntries);
        if (property.Hint is PropertyHint.ArrayType or PropertyHint.TypeString && strings.Length > 0)
        {
            var type = (Variant.Type)(strings[0].TryParseInt() ?? 0);
            var nextHint = strings.SelectElementAt(1, val => (PropertyHint)(val.TryParseInt() ?? 0));
            var nextHintString = strings.ElementAtOr(2, "");
            var nextProperty = JSON.FromProperty(new Property(name: "element", type, nextHint, nextHintString));
            return new Array(nextProperty, property.Default);
        }
        return new Array(null, property.Default);
    }

    public GodotDictionary GetSchema()
    {
        var dict = new GodotDictionary { { "type", "array" } };
        if (ElementSchema is not null) dict["items"] = ElementSchema.GetSchema();
        return this.WithDefault<Array, GodotArray>(dict);
    }

    public bool IsValid(Variant data)
    {
        if (data.Obj is not IEnumerable) return false;
        return ElementSchema is null || data.AsGodotArray().All(ElementSchema.IsValid);
    }

    public Variant Clean(Variant data)
    {
        if (!IsValid(data)) throw new InvalidDataException(this, data);
        if (ElementSchema is not null) return data.AsGodotArray().Select(data1 => ElementSchema.Clean(data1)).ToGodotArray();
        return data.AsGodotArray();
    }
}
