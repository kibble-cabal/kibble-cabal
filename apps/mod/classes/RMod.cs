using Godot;

[GlobalClass]
public partial class RMod : ExtensibleResource, IIdentifiable<StringName>
{
    private string _zipPath = "";
    private StringName _id = "";
    private string _author = "";
    private string _link = "";
    private string _version = "";
    private string _iconPath = "";
    private string _displayName = "";
    private string _displayDescription = "";
    private string _entryScriptPath = "";

    private IContentLoader Loader;

    [Export(PropertyHint.File, "*.zip")]
    public string ZIPPath
    {
        get => _zipPath;
        set
        {
            this.Set(ref _zipPath, value);
            Loader = new ContentLoader.ZIP(ZIPPath);
        }
    }

    [Export]
    public StringName ID
    {
        get => _id;
        set => this.Set(ref _id, value);
    }

    [Export]
    public string Author
    {
        get => _author;
        set => this.Set(ref _author, value);
    }

    [Export]
    public string Link
    {
        get => _link;
        set => this.Set(ref _link, value);
    }

    [Export]
    public string Version
    {
        get => _version;
        set => this.Set(ref _version, value);
    }

    [Export(PropertyHint.File, "*.png,*.jpg")]
    public string IconPath
    {
        get => _iconPath;
        set => this.Set(ref _iconPath, value);
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

    [Export(PropertyHint.File, "*.gd,*.cs")] // TODO change file type hint
    public string EntryScriptPath
    {
        get => _entryScriptPath;
        set => this.Set(ref _entryScriptPath, value);
    }

    public bool HasZIP => ZIPPath.Length > 0;
    public bool HasEntryScript => EntryScriptPath.Length > 0;
    public bool HasIcon => IconPath.Length > 0;

    public RMod()
    {
        Loader = new ContentLoader.ZIP(ZIPPath);
    }

    public Texture2D? GetIcon() => HasZIP && HasIcon ? Loader.LoadImage(IconPath) : null;

    public void RunEntryScript()
    {
        if (!HasZIP || !HasEntryScript) return;
        throw new System.NotImplementedException();
    }
    
    static RMod()
    {
        #if TOOLS
        JSONSchema.GeneratorDB.Register(new JSONSchema.Generator
        {
            ClassName = nameof(RMod),
            Path = "res://docs/schemas/Mod.schema.json",
            Title = "Mod"
        });
        #endif
    }
}