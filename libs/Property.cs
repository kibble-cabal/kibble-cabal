using Godot;
using Godot.Collections;

public record struct Property
{
    public StringName? Name;
    public readonly Variant.Type Ty = Variant.Type.Nil;
    public readonly PropertyHint Hint = PropertyHint.None;
    public string HintString = "";
    public PropertyUsageFlags Usage = PropertyUsageFlags.Default;
    public Variant? Default = null;

    public Property(
        StringName? name,
        Variant.Type type = Variant.Type.Nil,
        PropertyHint hint = PropertyHint.None,
        string hintString = "",
        PropertyUsageFlags usage = PropertyUsageFlags.Default,
        Variant? defaultValue = null
    )
    {
        this.Name = name;
        this.Ty = type;
        this.Hint = hint;
        this.HintString = hintString;
        this.Usage = usage;
        this.Default = defaultValue;
    }

    public static Property Group(StringName name, string prefix = "") => new Property
    {
        Name = name,
        HintString = prefix,
        Usage = PropertyUsageFlags.Group,
    };

    public static Property From(Dictionary prop) => new(
        name: prop.Get<StringName>("name", ""),
        type: prop.Get("type", Variant.Type.Nil),
        hint: prop.Get("hint", PropertyHint.None),
        hintString: prop.Get("hint_string", ""),
        usage: prop.Get("usage", PropertyUsageFlags.None),
        defaultValue: prop.Get("default")
    );

    public static implicit operator Dictionary(Property prop) => new Dictionary {
        { "name", prop.Name ?? "" },
        { "type", (int)prop.Ty },
        { "hint", (int)prop.Hint },
        { "hint_string", prop.HintString },
        { "usage", (int)prop.Usage },
        { "default", prop.Default ?? new Variant() }
    };
}