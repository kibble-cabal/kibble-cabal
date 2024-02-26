using Godot;

public partial class RQuest : ExtensibleResource, IIdentifiable<StringName>
{
    private StringName _id = "";
    private string _displayName = "";
    private string _displayDescription = "";
    private PackedScene? _uiScene;
    private Script? _script;

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
    public PackedScene? UIScene
    {
        get => _uiScene;
        set => this.Set(ref _uiScene, value);
    }

    /// <summary>
    /// Should implement IQuestScript.
    /// </summary>
    [Export]
    public Script? Script
    {
        get => _script;
        set => this.Set(ref _script, value);
    }

    public bool IsAvailable() => Script?.New<IQuestScript>()?.IsAvailable() ?? false;
    public bool IsComplete() => Script?.New<IQuestScript>()?.IsComplete() ?? false;
    public void Complete() => Script?.New<IQuestScript>()?.Complete();
    
    static RQuest()
    {
        #if TOOLS
        JSONSchema.GeneratorDB.Register(new JSONSchema.Generator
        {
            ClassName = nameof(RQuest),
            Path = "res://docs/schemas/Quest.schema.json",
            Title = "Quest"
        });
        #endif
    }
}