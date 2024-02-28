using System;
using System.Numerics;
using Godot;

using GodotDictionary = Godot.Collections.Dictionary;

namespace JSON.Schema.Type;

public class Number<[MustBeVariant] T> : IJSONType where T: struct, INumber<T>
{
    public T? Min;
    public T? Max;
    public T? Step;
    public Variant? Default { get; set; }
    
    public static Number<T> FromProperty(Property property)
    {
        var strs = property.HintString.Split(",", StringSplitOptions.RemoveEmptyEntries);
        T? min = null, max = null, step = null;
        if (strs.Has(0) && !property.HintString.Contains("or_less"))
            min = strs[0].TryParse<T>();
        if (strs.Has(1) && !property.HintString.Contains("or_greater"))
            max = strs[1].TryParse<T>();
        if (strs.Has(2))
            step = strs[2].TryParse<T>();
        return new Number<T> { Min = min, Max = max, Step = step, Default = property.Default };
    }
    
    public GodotDictionary GetSchema()
    {
        var dict = new GodotDictionary { { "type", typeof(T) == typeof(int) ? "integer" : "number" } };
        if (Min is { } min) dict["minimum"] = Variant.From(min);
        if (Max is { } max) dict["maximum"] = Variant.From(max);
        if (Step is { } step) dict["step"] = Variant.From(step);
        return this.WithDefault<Number<T>, T>(dict);
    }

    public bool IsValid(Variant data)
    {
        if (typeof(T) == typeof(float)) return data.VariantType is Variant.Type.Float or Variant.Type.Int;
        if (typeof(T) == typeof(int)) return data.VariantType == Variant.Type.Int;
        return false;
    }
    
    public Variant Clean(Variant data) => IsValid(data) ? Variant.From(data.As<T>()) : throw new InvalidDataException(this, data);
}
