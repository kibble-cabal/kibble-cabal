using System.Collections.Generic;
using Godot;
using Godot.Collections;

[GlobalClass]
public sealed partial class RSave : ExtensibleResource, IIdentifiable<int>
{
    private static class Keys
    {
        public const string Settings = "Settings";
        public const string Fate = "Fate";
        public const string DateTime = "DateTime";
    }

    public const string BaseDir = "user://save-files";

    private int _id = -1;
    private Array<RLocationState> _locationStates = [];

    private double SessionStartTime = Time.GetUnixTimeFromSystem();

    [Export]
    public int ID
    {
        get => _id;
        private set => this.Set(ref _id, value);
    }

    [Export]
    public Array<RLocationState> LocationStates
    {
        get => _locationStates;
        set
        {
            this.Set(ref _locationStates, value);
            ConnectAllSubResources();
        }
    }

    [Export]
    public string LastSaved = "";

    [Export]
    public float TimePlayed = 0.0f;

    public RSettings Settings
    {
        get => ExpectSubResource<RSettings>(Keys.Settings);
        set => SetSubresource(Keys.Settings, value);
    }

    public RFate Fate
    {
        get => ExpectSubResource<RFate>(Keys.Fate);
        set => SetSubresource(Keys.Fate, value);
    }

    public RDateTime DateTime
    {
        get => ExpectSubResource<RDateTime>(Keys.DateTime);
        set => SetSubresource(Keys.DateTime, value);
    }

    [Signal]
    public delegate void BeforeSavedEventHandler();

    [Signal]
    public delegate void AfterSavedEventHandler();

    private string FileName => $"Save-{ID}.tres";
    private string SavePath => $"{BaseDir}/{FileName}";

    protected override IEnumerable<Resource> _GetAllSubResources() => [.. base._GetAllSubResources(), .. _locationStates];

    public RLocationState GetOrCreateLocationState(StringName location)
    {
        if (LocationStates.Find(state => state.LocationName == location) is RLocationState state)
            return state;
        var newState = new RLocationState { LocationName = location };
        LocationStates.Add(newState);
        ConnectSubresource(newState);
        return newState;
    }

    private void GenerateID()
    {
        ID = DirAccess.DirExistsAbsolute(BaseDir) ? (DirAccess.GetFilesAt(BaseDir).Length + 1) : 1;
        DirAccess.MakeDirRecursiveAbsolute(BaseDir);
    }

    private void UpdateTimePlayed()
    {
        var now = (float)Time.GetUnixTimeFromSystem();
        var sessionLength = now - SessionStartTime;
        TimePlayed += (float)sessionLength;
        SessionStartTime = now;
    }

    public void CommitChanges()
    {
        EmitSignal(SignalName.BeforeSaved);
        UpdateTimePlayed();
        if (ID < 0) GenerateID();
        LastSaved = Time.GetDatetimeStringFromSystem();
        ResourceSaver.Save(this, SavePath);
        EmitSignal(SignalName.AfterSaved);
    }
    
    static RSave()
    {
        #if TOOLS
        JSONSchema.GeneratorDB.Register(new JSONSchema.Generator
        {
            ClassName = nameof(RSave),
            Path = "res://docs/schemas/Save.schema.json",
            Title = "Save"
        });
        #endif
    }
}