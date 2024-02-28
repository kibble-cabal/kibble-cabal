using Godot;

[GlobalClass]
public partial class RLocation : ExtensibleResource, IIdentifiable<StringName>
{
    private StringName _id = "";
    private PackedScene? _map;
    private Vector3 _cameraSpawnPosition;
    private StringName _musicID = "";

    public StringName ID => _id;

    [Export]
    public StringName Name
    {
        get => _id;
        set => this.Set(ref _id, value);
    }

    [Export]
    public PackedScene? Map
    {
        get => _map;
        set => this.Set(ref _map, value);
    }

    [Export]
    public Vector3 CameraSpawnPosition
    {
        get => _cameraSpawnPosition;
        set => this.Set(ref _cameraSpawnPosition, value);
    }

    [Export]
    public StringName MusicID
    {
        get => _musicID;
        set => this.Set(ref _musicID, value);
    }

    public RMusic? GetMusic() => MusicDB.Instance.Find(MusicID);

    public RLocationState GetOrCreateState() => SaveSubSystem.Current?.GetOrCreateLocationState(ID)
        ?? new RLocationState { LocationName = Name };
    
    static RLocation()
    {
        #if TOOLS
        JSON.Schema.GeneratorDB.Register(new JSON.Schema.Generator
        {
            ClassName = nameof(RLocation),
            Path = "res://docs/schemas/Location.schema.json",
            Title = "Location"
        });
        #endif
    }
}