using System.Linq;
using Godot;

public sealed partial class LocationSubSystemBase : Node, ISaveFileSubSystem
{
    public StringName CurrentLocation { get; private set; } = "";
    public Node3D? CurrentMap { get; private set; } = null;

    [Signal]
    public delegate void LocationEnteredEventHandler(RLocation location);

    [Signal]
    public delegate void LocationExitedEventHandler(RLocation location);

    [Signal]
    public delegate void LocationChangedEventHandler();

    public RLocationState? GetState() => GetLocation()?.GetOrCreateState();

    public RLocation? GetLocation() => LocationDB.Instance.Find(CurrentLocation);

    public Node3D? GetRoot() => GetTree().GetFirstNodeInGroup("LocationRoot") as Node3D;

    public void To(StringName name) => To(LocationDB.Instance.Find(name));

    public void To(RLocation? location)
    {
        Exit();
        Enter(location);
        EmitSignal(SignalName.LocationChanged);
    }

    private void Enter(RLocation? location)
    {
        if (location == null) return;
        CurrentLocation = location.Name;
        SpawnMap();
        SpawnSpawners();
        EmitSignal(SignalName.LocationEntered, [location]);
        GD.Print($"[LocationSystem] Entering location: {CurrentLocation}");
    }

    private void Exit()
    {
        RLocation? location = GetLocation();
        DespawnSpawners();
        DespawnMap();
        CurrentLocation = "";
        CurrentMap = null;
        if (location is RLocation value)
            EmitSignal(SignalName.LocationExited, [value]);
    }

    private void SpawnMap()
    {
        if (GetLocation() is RLocation location && GetRoot() is Node3D root && location.Map != null)
        {
            CurrentMap = location.Map.Instantiate() as Node3D;
            root.AddChild(CurrentMap);
            root.MoveChild(CurrentMap, 0);
        }
    }

    private void SpawnSpawners()
    {
        if (GetState() is RLocationState state && CurrentMap != null)
        {
            state.TryConnect(RLocationState.SignalName.SpawnersChanged, Callable.From(OnSpawnersChanged));
            state.Spawners.ForEach(spawner => spawner.Spawn(CurrentMap));
        }
    }

    private void OnSpawnersChanged()
    {
        if (GetState() is RLocationState state && CurrentMap != null)
        {
            // Remove outdated spawners
            GetTree()
                .GetNodesInGroup(Spawner.TopLevelGroupName)
                .Select(spawner => (Spawner)spawner.GetMeta(Spawner.MetaName))
                .Except(state.Spawners)
                .ForEach(spawner => spawner.Despawn());

            // Add new spawners
            state.GetUnspawned().ForEach(spawner => spawner.Spawn(CurrentMap));
        }
    }

    private void DespawnMap()
    {
        if (CurrentMap.CanQueueFree())
            CurrentMap?.QueueFree();
    }

    private void DespawnSpawners()
    {
        if (GetState() is RLocationState state)
            state.GetSpawned().ForEach(spawner => spawner.Despawn());
    }
}

public sealed class LocationSubSystem : Singleton<LocationSubSystemBase>
{
    public static RLocationState? GetState() => Instance.GetState();
    public static RLocation? GetLocation() => Instance.GetLocation();
    public static Node3D? GetRoot() => Instance.GetRoot();
    public static void To(StringName name) => Instance.To(name);
    public static void To(RLocation location) => Instance.To(location);
}