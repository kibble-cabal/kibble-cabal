using System.Linq;
using Godot;

using Godot.Collections;
using Collections = System.Collections.Generic;

using Ty = System.Type;

[GlobalClass]
public partial class RBuilding : Resource
{
    public const int TessellationStages = 3;
    public const float TessellationToleranceDegrees = 3;

    /* Private properties */

    public Collections.List<Wall> Walls = [];
    public Collections.List<Floor> Floors = [];
    public Collections.List<Roof> Roofs = [];
    public Callable ChangedCallable;

    public RBuilding() => ChangedCallable = Callable.From(EmitChanged);

    /* Public properties */

    [Export]
    public Array WallData
    {
        get => Walls.Select(wall => (Variant)wall.Serialize()).ToGodotArray();
        set
        {
            Walls.Clear();
            Walls.AddRange(value.Select(val => Wall.Deserialize(val.As<Array>())).WhereOK());
        }
    }

    [Export]
    public Array FloorData
    {
        get => Floors.Select(floor => (Variant)floor.Serialize()).ToGodotArray();
        set
        {
            Floors.Clear();
            Floors.AddRange(value.Select(val => Floor.Deserialize(val.As<Array>())).WhereOK());
            Floors.ForEach(floor => floor.Polygon.TryConnectChanged(ChangedCallable));
        }
    }

    [Export]
    public Array RoofData
    {
        get => Roofs.Select(roof => (Variant)roof.Serialize()).ToGodotArray();
        set
        {
            Roofs.Clear();
            Roofs.AddRange(value.Select(val => Roof.Deserialize(val.As<Array>())).WhereOK());
            Roofs.ForEach(roof => roof.Polygon.TryConnectChanged(ChangedCallable));
        }
    }

    /* Public signals */

    [Signal]
    public delegate void WallAddedEventHandler(int index);

    [Signal]
    public delegate void WallRemovedEventHandler(int index, Array data);

    [Signal]
    public delegate void FloorAddedEventHandler(int index);

    [Signal]
    public delegate void FloorRemovedEventHandler(int index, Array data);

    [Signal]
    public delegate void RoofAddedEventHandler(int index);

    [Signal]
    public delegate void RoofRemovedEventHandler(int index, Array data);

    [Signal]
    public delegate void DestroyRequestedEventHandler();

    [Signal]
    public delegate void MoveRequestedEventHandler();

    [Signal]
    public delegate void EditRequestedEventHandler();

    [Signal]
    public delegate void DestroyWallRequestedEventHandler(int index);

    [Signal]
    public delegate void DestroyFloorRequestedEventHandler(int index);

    [Signal]
    public delegate void MoveWallRequestedEventHandler(int index);

    [Signal]
    public delegate void MoveFloorRequestedEventHandler(int index);

    static internal StringName AddSignalName<T>() where T : IBuildingComponent<T> => typeof(T) switch
    {
        Ty t when t == typeof(Wall) => nameof(WallAdded),
        Ty t when t == typeof(Floor) => nameof(FloorAdded),
        Ty t when t == typeof(Roof) => nameof(RoofAdded),
        _ => ""
    };

    static internal StringName RemoveSignalName<T>() where T : IBuildingComponent<T> => typeof(T) switch
    {
        Ty t when t == typeof(Wall) => nameof(WallRemoved),
        Ty t when t == typeof(Floor) => nameof(FloorRemoved),
        Ty t when t == typeof(Roof) => nameof(RoofRemoved),
        _ => ""
    };

    // Unsafe and gross! Use with caution!
    internal Collections.List<T> GetList<T>() where T : IBuildingComponent<T> => typeof(T) switch
    {
        Ty t when t == typeof(Wall) => (Collections.List<T>)System.Convert.ChangeType(Walls, typeof(Collections.List<T>)),
        Ty t when t == typeof(Floor) => (Collections.List<T>)System.Convert.ChangeType(Floors, typeof(Collections.List<T>)),
        Ty t when t == typeof(Roof) => (Collections.List<T>)System.Convert.ChangeType(Roofs, typeof(Collections.List<T>)),
        _ => []
    };

    internal int Count<T>() where T : IBuildingComponent<T> => typeof(T) switch
    {
        Ty t when t == typeof(Wall) => Walls.Count,
        Ty t when t == typeof(Floor) => Floors.Count,
        Ty t when t == typeof(Roof) => Roofs.Count,
        _ => 0
    };
}