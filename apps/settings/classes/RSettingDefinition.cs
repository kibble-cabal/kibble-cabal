using Godot;

[GlobalClass]
public sealed partial class RSettingDefinition : ExtensibleResource, IIdentifiable<StringName>
{
    private StringName _id = "";
    private string _displayName = "";
    private string _displayDescription = "";
    private PackedScene? _ui;

    [Export]
    public StringName ID
    {
        get => _id;
        set => this.Set(ref _id, value);
    }

    [Export]
    public string DisplayName
    {
        get => _displayName;
        set => this.Set(ref _displayName, value);
    }

    [Export(PropertyHint.MultilineText)]
    public string DisplayDescription
    {
        get => _displayDescription;
        set => this.Set(ref _displayDescription, value);
    }

    [Export]
    public PackedScene? UI
    {
        get => _ui;
        set => this.Set(ref _ui, value);
    }
    
    static RSettingDefinition()
    {
        #if TOOLS
        JSON.Schema.GeneratorDB.Register(new JSON.Schema.Generator
        {
            ClassName = nameof(RSettingDefinition),
            Path = "res://docs/schemas/SettingDefinition.schema.json",
            Title = "Setting Definition"
        });
        #endif
    }
}