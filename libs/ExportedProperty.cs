using Godot;
using Godot.Collections;

public record struct ExportedProperty
{
    public StringName Name;
    public Variant.Type Ty = Variant.Type.Nil;
    public PropertyHint Hint = PropertyHint.None;
    public string HintString = "";
    public PropertyUsageFlags Usage = PropertyUsageFlags.Default;

    public ExportedProperty(
        StringName name,
        Variant.Type type = Variant.Type.Nil,
        PropertyHint hint = PropertyHint.None,
        string hintString = "",
        PropertyUsageFlags usage = PropertyUsageFlags.Default
    )
    {
        this.Name = name;
        this.Ty = type;
        this.Hint = hint;
        this.HintString = hintString;
        this.Usage = usage;
    }

    public static ExportedProperty Group(StringName name, string prefix = "") => new ExportedProperty
    {
        Name = name,
        HintString = prefix,
        Usage = PropertyUsageFlags.Group,
    };

    public static implicit operator Dictionary(ExportedProperty prop) => new Dictionary {
        { "name", prop.Name },
        { "type", (int)prop.Ty },
        { "hint", (int)prop.Hint },
        { "hint_string", prop.HintString },
        { "usage", (int)prop.Usage }
    };
}