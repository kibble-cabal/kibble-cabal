using System;
using System.Linq;
using Godot;

using GodotDictionary = Godot.Collections.Dictionary;

namespace JSON.Schema.Type;

internal class JSON : IJSONType
{
    public System.Collections.Generic.Dictionary<StringName, IJSONType> Properties = [];
    public StringName[] RequiredProperties = [];
    public bool AdditionalProperties = true;
    public Variant? Default { get; set; }
    public bool ConvertCase = true;

    public static IJSONType FromProperty(Property property)
    {
        return property.Ty switch
        {
            Variant.Type.Array
                or Variant.Type.PackedByteArray
                or Variant.Type.PackedInt32Array
                or Variant.Type.PackedInt64Array
                or Variant.Type.PackedFloat32Array
                or Variant.Type.PackedFloat64Array
                or Variant.Type.PackedStringArray
                or Variant.Type.PackedColorArray
                or Variant.Type.PackedVector2Array
                or Variant.Type.PackedVector3Array => Array.FromProperty(property),
            Variant.Type.Bool => Boolean.FromProperty(property),
            Variant.Type.Dictionary => Dict.FromProperty(property),
            Variant.Type.Float => Number<float>.FromProperty(property),
            Variant.Type.Int => Number<int>.FromProperty(property),
            Variant.Type.String or Variant.Type.StringName => String.FromProperty(property),
            Variant.Type.Vector2
                or Variant.Type.Vector2I
                or Variant.Type.Vector3
                or Variant.Type.Vector3I
                or Variant.Type.Vector4
                or Variant.Type.Vector4I
                or Variant.Type.Color => Vector.FromProperty(property),
            Variant.Type.Rect2 or Variant.Type.Rect2I => Rect.FromProperty(property),
            Variant.Type.Object => GodotResource.FromProperty(property),
            _ => throw new NotImplementedException($"Not implemented: {property.Name} ({property.Ty})")
        };
    }

    public GodotDictionary GetSchema()
    {
        var propDict = new GodotDictionary();
        foreach (var propName in Properties.Keys)
            propDict[ConvertCase ? propName.Pascal() : propName] = Properties[propName].GetSchema();
        return new GodotDictionary
        {
            { "type", "object" },
            { "properties", propDict },
            { "required", RequiredProperties },
            { "additionalProperties", AdditionalProperties }
        };
    }
    
    public bool IsValid(Variant data)
    {
        if (data.Obj is not GodotDictionary) return false;
        var dict = data.AsGodotDictionary();
        var propNames = Properties.Select(prop => (string)prop.Key).Order();
        var dataPropNames = dict.Keys.Select(prop => prop.AsString()).Order();

        // Check for extra properties
        if (!AdditionalProperties && !dataPropNames.SequenceEqual(propNames)) return false;
        
        // Check for required properties
        if (!RequiredProperties.All(prop => dict.ContainsKey((string)prop))) return false;
        
        // Check that values are valid
        return propNames.Intersect(dataPropNames).All(propName => Properties[propName].IsValid(dict[propName]));
    }

    public Variant Clean(Variant data)
    {
        if (!IsValid(data)) throw new InvalidDataException(this, data);
        var dict = data.AsGodotDictionary();
        var newKeys = dict.Keys.Select(key => key.AsStringName());
        var newData = newKeys.Intersect(Properties.Keys).Select(key => Properties[key].Clean(dict[key]));
        return newKeys.Zip(newData).ToGodotDictionary();
    }
}