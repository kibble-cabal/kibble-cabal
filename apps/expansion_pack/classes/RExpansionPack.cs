using Godot;

[GlobalClass]
public partial class RExpansionPack : Resource, IIdentifiable<StringName>
{
    [Export]
    public StringName ID { get; set; } = "";

    [Export]
    public string DisplayName = "";

    [Export(PropertyHint.MultilineText)]
    public string DisplayDescription = "";

    [Export]
    public Texture2D? Icon;

    [Export]
    public string Version = "";

    [Export]
    public Script? EntryScript;
}