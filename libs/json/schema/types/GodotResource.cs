using Godot;
using BB;

using GodotDictionary = Godot.Collections.Dictionary;

namespace JSON.Schema.Type;

internal class GodotResource : IJSONType
{
    private GodotClass? GodotClass;
    public Variant? Default { get; set; }

    public static GodotResource FromProperty(Property property)
    {
        var obj = new GodotResource(); // TODO default
        if (property.Hint is PropertyHint.ResourceType)
            obj.GodotClass = new GodotClass(property.HintString);
        return obj;
    }

    public GodotDictionary GetSchema()
    {
        var generator = GodotClass?.FindGenerator();
        if (generator is not null)
            return new GodotDictionary { { "$ref", generator.ID } }; // TODO default
        GD.PrintRich($"[Warning] Cannot reference Godot class {GodotClass} in schema.".Yellow());
        return new GodotDictionary { { "$ref", "unknown" } };
    }

    public bool IsValid(Variant data)
    {
        throw new System.NotImplementedException();
    }

    public Variant Clean(Variant data)
    {
        throw new System.NotImplementedException();
    }
}