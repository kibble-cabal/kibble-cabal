using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class RLocationState : ExtensibleResource
{
    private StringName _locationName = "";

    [Export]
    public StringName LocationName
    {
        get => _locationName;
        set => this.Set(ref _locationName, value);
    }

    [Export]
    public Array<Spawner> Spawners { get; private set; } = [];

    [Signal]
    public delegate void SpawnersChangedEventHandler();

    public RLocation? GetLocation() => LocationDB.Instance.Find(LocationName);

    public void Add<S>(S spawner) where S : Spawner
    {
        if (Spawners.Contains(spawner)) return;
        Spawners.Add(spawner);
        EmitSignal(SignalName.SpawnersChanged);
        EmitChanged();
    }

    public void Remove<S>(S spawner) where S : Spawner
    {
        if (!Spawners.Contains(spawner)) return;
        Spawners.Remove(spawner);
        EmitSignal(SignalName.SpawnersChanged);
        EmitChanged();
    }

    public IEnumerable<Spawner> GetSpawned() => Spawners.Where(spawner => spawner.HasSpawned());
    public IEnumerable<Spawner> GetUnspawned() => Spawners.Where(spawner => !spawner.HasSpawned());
    public bool HasSpawnerFor<R>(R resource) where R : Resource => Spawners.Any(spawner => spawner.GetResource() == resource);
    public void RemoveSpawnersFor<R>(R resource) where R : Resource => Spawners.Where(spawner => spawner.GetResource() == resource).ForEach(Remove<Spawner>);

    protected override IEnumerable<Resource> _GetAllSubresources() => [.. base._GetAllSubresources(), .. Spawners];
}