using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class RLocationState : ExtensibleResource
{
    private StringName _locationName = "";
    private Transform3D _cameraTransform = Transform3D.Identity;
    private float _cameraZoom = 35.0f;

    [Export]
    public StringName LocationName
    {
        get => _locationName;
        set => this.Set(ref _locationName, value);
    }

    [Export]
    public Transform3D CameraTransform
    {
        get => _cameraTransform;
        set => this.Set(ref _cameraTransform, value);
    }

    [Export]
    public float CameraZoom
    {
        get => _cameraZoom;
        set => this.Set(ref _cameraZoom, value);
    }

    [Export(PropertyHint.ArrayType, nameof(SpawnerBase))]
    public Array<SpawnerBase> Spawners { get; private set; } = [];

    [Signal]
    public delegate void SpawnersChangedEventHandler();

    public RLocation? GetLocation() => LocationDB.Instance.Find(LocationName);

    public void Add<S>(S spawner) where S : SpawnerBase
    {
        if (Spawners.Contains(spawner)) return;
        Spawners.Add(spawner);
        EmitSignal(SignalName.SpawnersChanged);
        EmitChanged();
    }

    public void Remove<S>(S spawner) where S : SpawnerBase
    {
        if (!Spawners.Contains(spawner)) return;
        Spawners.Remove(spawner);
        EmitSignal(SignalName.SpawnersChanged);
        EmitChanged();
    }

    public IEnumerable<S> Get<S>() where S : SpawnerBase => Spawners.Where(spawner => spawner.GetType() == typeof(S)).Select(spawner => (S)spawner);
    public IEnumerable<SpawnerBase> GetSpawned() => Spawners.Where(spawner => spawner.HasSpawned());
    public IEnumerable<SpawnerBase> GetUnspawned() => Spawners.Where(spawner => !spawner.HasSpawned());
    public bool HasSpawnerFor<R>(R resource) where R : Resource => Spawners.Any(spawner => spawner.GetResource() == resource);
    public void RemoveSpawnersFor<R>(R resource) where R : Resource => Spawners.Where(spawner => spawner.GetResource() == resource).ForEach(Remove<SpawnerBase>);

    protected override IEnumerable<Resource> _GetAllSubResources() => [.. base._GetAllSubResources(), .. Spawners];
    
    static RLocationState()
    {
        #if TOOLS
        JSON.Schema.GeneratorDB.Register(new JSON.Schema.Generator
        {
            ClassName = nameof(RLocationState),
            Path = "res://docs/schemas/LocationState.schema.json",
            Title = "Location State"
        });
        #endif
    }
}