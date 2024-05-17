using System.Linq;
using Godot;

public sealed partial class LocationSubSystemBase : Node, ISubSystem, ISaveFileSubSystem
{
    [Export]
    public StringName CurrentLocation { get; private set; } = "";

    [Export]
    public Node3D? CurrentMap { get; private set; } = null;

    [Signal]
    public delegate void LocationEnteredEventHandler(RLocation location);

    [Signal]
    public delegate void LocationExitedEventHandler(RLocation location);

    [Signal]
    public delegate void LocationChangedEventHandler();

    public LocationSubSystemBase() => this.Name = "LocationSubSystem";

    public RLocationState? GetState() => GetLocation()?.GetOrCreateState();

    public RLocation? GetLocation() => LocationDB.Instance.Find(CurrentLocation);

    public Node3D? GetRoot() => this.GetLocationRoot();

    public Camera3D? GetCamera() => GetRoot()?.GetViewport().GetCamera3D();

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

    public override void _Ready()
    {
        var timer = new Timer();
        timer.WaitTime = 3.0;
        timer.Autostart = true;
        timer.Timeout += UpdateCameraTransform;
        AddChild(timer);
    }

    private void SpawnMap()
    {
        var location = GetLocation();
        if (location is null) return;
        
        if (GetRoot() is Node3D root && location.Map != null)
        {
            CurrentMap = location.Map.Instantiate() as Node3D;
            root.AddChild(CurrentMap);
            root.MoveChild(CurrentMap, 0);
        }

        if (GetCamera() is Camera3D camera)
        {
            var state = location.GetOrCreateState();
            camera.Transform = state.CameraTransform;
            camera.Fov = state.CameraZoom;
            camera.Set("target_position", camera.Transform.Origin);
            camera.Set("target_rotation", camera.Transform.Basis.GetRotationQuaternion());
            camera.Set("target_zoom", camera.Fov);
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
                .GetNodesInGroup(SpawnerBase.TopLevelGroupName)
                .Select(spawner => (SpawnerBase)spawner.GetMeta(SpawnerBase.MetaName))
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

    private void UpdateCameraTransform()
    {
        if (GetCamera() is Camera3D camera && GetState() is RLocationState locationState)
        {
            locationState.CameraTransform = camera.Transform;
            locationState.CameraZoom = camera.Fov;
        }
    }

    public RPet[] GetPets() => [.. GetPetSpawners().Select(pet => pet.GetResource())];
    public RPetSpawner[] GetPetSpawners() => [.. GetState()?.Get<RPetSpawner>() ?? []];
}

public sealed class LocationSubSystem : Singleton<LocationSubSystemBase>
{
    public static RLocationState? GetState() => Instance.GetState();
    public static RPet[] GetPets() => Instance.GetPets();
    public static RPetSpawner[] GetPetSpawners() => Instance.GetPetSpawners();
    public static RLocation? GetLocation() => Instance.GetLocation();
    public static Node3D? GetRoot() => Instance.GetRoot();
    public static void Add<S>(S spawner) where S : SpawnerBase => GetState()?.Add<S>(spawner);
    public static void To(StringName name) => Instance.To(name);
    public static void To(RLocation location) => Instance.To(location);
}