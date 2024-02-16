using System.Linq;
using Godot;

using Godot.Collections;
using Collections = System.Collections.Generic;

#nullable enable

using Ty = System.Type;

[GlobalClass]
public partial class Building : Resource
{
    public const int TessellationStages = 3;
    public const float TessellationToleranceDegrees = 3;

    /* Private properties */

    public Collections.List<Wall> Walls = [];
    public Collections.List<Floor> Floors = [];
    public Collections.List<Roof> Roofs = [];
    public Callable ChangedCallable;

    Building() => this.ChangedCallable = Callable.From(EmitChanged);

    /* Public properties */

    [Export]
    public Array wall_data
    {
        get => Walls.Select(wall => (Variant)wall.Serialize()).ToGodotArray();
        set
        {
            Walls.Clear();
            Walls.AddRange(value.Select(val => Wall.Deserialize(val.As<Array>())).WhereOK());
        }
    }

    public int wall_count => Walls.Count;

    [Export]
    public Array floor_data
    {
        get => Floors.Select(floor => (Variant)floor.Serialize()).ToGodotArray();
        set
        {
            Floors.Clear();
            Floors.AddRange(value.Select(val => Floor.Deserialize(val.As<Array>())).WhereOK());
            Floors.ForEach(floor => floor.Polygon.TryConnectChanged(ChangedCallable));
        }
    }

    public int floor_count => Floors.Count;

    [Export]
    public Array roof_data
    {
        get => Roofs.Select(roof => (Variant)roof.Serialize()).ToGodotArray();
        set
        {
            Roofs.Clear();
            Roofs.AddRange(value.Select(val => Roof.Deserialize(val.As<Array>())).WhereOK());
            Roofs.ForEach(roof => roof.Polygon.TryConnectChanged(ChangedCallable));
        }
    }

    public int roof_count => Roofs.Count;

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

    /* Public wall methods */

    private WallRef? get_wall(int index) => this.GetWallRef(index);
    private Array get_wall_data(int index) => this.wall_data[index].As<Array>();
    private void remove_invalid_walls() => this.RemoveInvalid<Wall>();
    private bool has_wall(int index) => this.Has<Wall>(index);
    private bool is_wall_valid(int index) => this.IsValid<Wall>(index);
    private int add_wall() => this.Add<Wall>(Vector2.Inf, Vector2.Inf);
    private int add_wall(Vector2 start, Vector2 end) => this.Add<Wall>(start, end);
    private int add_wall(Vector2 start, Vector2 end, Vector2 start_handle, Vector2 end_handle) => this.Add<Wall>(start, end, start_handle, end_handle);
    private int add_wall(Array data) => this.Add<Wall>(data);
    private int[] add_walls(Vector2[] points) => this.Add<Wall>(points);
    private int[] add_walls(Curve2D curve) => this.Add<Wall>(curve);
    private void insert_wall(int index, Array data) => this.Insert<Wall>(index, data);
    private Vector2 get_wall_start(int index) => this.GetWallStart(index);
    private Vector2 get_wall_end(int index) => this.GetWallEnd(index);
    private Vector2 get_wall_start_handle(int index) => this.GetWallStartHandle(index);
    private Vector2 get_wall_end_handle(int index) => this.GetWallEndHandle(index);
    private float get_wall_height(int index) => this.GetWallHeight(index);
    private float get_wall_thickness(int index) => this.GetWallThickness(index);
    private Vector2 get_wall_midpoint(int index) => this.GetWallMidpoint(index);
    private void fill_wall_height(int index, float value) => this.FillWallHeight(index, value);
    private void fill_wall_thickness(int index, float value) => this.FillWallThickness(index, value);
    private void set_wall_positions(int index, Vector2 start, Vector2 end) => this.SetWallPositions(index, start, end);
    private void set_wall_handles(int index, Vector2 start_handle, Vector2 end_handle) => this.SetWallHandles(index, start_handle, end_handle);
    private void set_wall(int index, Vector2 start, Vector2 start_handle, Vector2 end, Vector2 end_handle) => this.SetWall(index, start, start_handle, end, end_handle);
    private void set_wall_start(int index, Vector2 position) => this.SetWallStart(index, position);
    private void set_wall_end(int index, Vector2 position) => this.SetWallEnd(index, position);
    private void set_wall_start_handle(int index, Vector2 position) => this.SetWallStartHandle(index, position);
    private void set_wall_end_handle(int index, Vector2 position) => this.SetWallEndHandle(index, position);
    private void set_wall_height(int index, float value) => this.SetWallHeight(index, value);
    private void set_wall_thickness(int index, float value) => this.SetWallThickness(index, value);
    private void move_wall_by(int index, Vector2 delta) => this.MoveBy<Wall>(index, delta);
    private void remove_wall(int index) => this.Remove<Wall>(index);
    private void remove_connected_walls(int index) => this.RemoveConnected<Wall>(index);
    private void move_connected_walls_by(int index, Vector2 delta) => this.MoveConnectedBy<Wall>(index, delta);
    private Dictionary<StringName, StringName> get_wall_materials(int index) => this.GetMaterials<Wall>(index) ?? new();
    private StringName get_wall_material_id(int index, StringName material_name) => this.GetMaterialID<Wall>(index, material_name) ?? new();
    private StringName get_wall_interior_id(int index) => this.GetWallInteriorID(index) ?? new();
    private StringName get_wall_exterior_id(int index) => this.GetWallExteriorID(index) ?? new();
    private void set_wall_materials(int index, Dictionary<StringName, StringName> value) => this.SetMaterials<Wall>(index, value);
    private void set_wall_material_id(int index, StringName material_name, StringName id) => this.SetMaterialID<Wall>(index, material_name, id);
    private void set_wall_interior_id(int index, StringName id) => this.SetWallInteriorID(index, id);
    private void set_wall_exterior_id(int index, StringName id) => this.SetWallExteriorID(index, id);
    private Vector2[] tessellate_wall(int index) => this.Tessellate<Wall>(index);
    private Vector2 snap_to_wall(int index, Vector2 position, float threshold) => this.Snap<Wall>(index, position, threshold);
    private Vector2 snap_to_wall(int index, Vector2 position) => this.Snap<Wall>(index, position);
    private Vector2 snap_to_wall_surface(int index, Vector2 position, float threshold) => this.SnapToSurface<Wall>(index, position, threshold);
    private Vector2 snap_to_wall_surface(int index, Vector2 position) => this.SnapToSurface<Wall>(index, position);
    private Vector2 snap_to_walls(Vector2 position, float threshold) => this.Snap<Wall>(position, threshold);
    private Vector2 snap_to_walls(Vector2 position) => this.Snap<Wall>(position);
    private Vector2 snap_to_walls_surface(Vector2 position, float threshold) => this.SnapToSurface<Wall>(position, threshold);
    private Vector2 snap_to_walls_surface(Vector2 position) => this.SnapToSurface<Wall>(position);
    private bool are_walls_touching(int a, int b) => this.IsTouching<Wall>(a, b);
    private int[] get_walls_touching(int wall_index) => this.GetIndicesTouching<Wall>(wall_index).ToArray();

    /* Public floor methods */

    private FloorRef? get_floor(int index) => this.GetFloorRef(index);
    private Array get_floor_data(int index) => this.floor_data[index].As<Array>();
    private void remove_invalid_floors() => this.RemoveInvalid<Floor>();
    private bool has_floor(int index) => this.Has<Floor>(index);
    private bool is_floor_valid(int index) => this.IsValid<Floor>(index);
    private int add_floor() => this.Add<Floor>(new Curve2D());
    private int add_floor(Curve2D polygon) => this.Add<Floor>(polygon);
    private int add_floor(Vector2[] points) => this.Add<Floor>(points);
    private void set_floor_polygon(int index, Curve2D polygon) => this.SetFloorPolygon(index, polygon);
    private void move_floor_by(int index, Vector2 delta) => this.MoveBy<Floor>(index, delta);
    private void remove_floor(int index) => this.Remove<Floor>(index);
    private float get_floor_thickness(int index) => this.GetFloorThickness(index);
    private void set_floor_thickness(int index, float value) => this.SetFloorThickness(index, value);
    private Dictionary<StringName, StringName> get_floor_materials(int index) => this.GetMaterials<Floor>(index) ?? new();
    private StringName get_floor_material_id(int index, StringName material_name) => this.GetMaterialID<Floor>(index, material_name) ?? new();
    private StringName get_floor_id(int index) => this.GetFloorID(index);
    private void set_floor_materials(int index, Dictionary<StringName, StringName> value) => this.SetMaterials<Floor>(index, value);
    private void set_floor_material_id(int index, StringName material_name, StringName id) => this.SetMaterialID<Floor>(index, material_name, id);
    private void set_floor_id(int index, StringName id) => this.SetFloorID(index, id);
    private Curve2D? get_floor_polygon(int index) => this.GetFloorPolygon(index);
    private Vector2[] tessellate_floor(int index) => this.Tessellate<Floor>(index);
    private Vector2 snap_to_floor(int index, Vector2 position, float threshold) => this.Snap<Floor>(index, position, threshold);
    private Vector2 snap_to_floor(int index, Vector2 position) => this.Snap<Floor>(index, position);
    private Vector2 snap_to_floor_surface(int index, Vector2 position, float threshold) => this.SnapToSurface<Floor>(index, position, threshold);
    private Vector2 snap_to_floor_surface(int index, Vector2 position) => this.SnapToSurface<Floor>(index, position, -1);
    private Vector2 snap_to_floors(Vector2 position, float threshold) => this.Snap<Floor>(position, threshold);
    private Vector2 snap_to_floors(Vector2 position) => this.Snap<Floor>(position);
    private Vector2 snap_to_floors_surface(Vector2 position, float threshold) => this.SnapToSurface<Floor>(position, threshold);
    private Vector2 snap_to_floors_surface(Vector2 position) => this.SnapToSurface<Floor>(position);
    private bool are_floors_touching(int a, int b, float threshold) => this.IsTouching<Floor>(a, b, threshold);
    private Vector2[] get_floor_point_positions(int index) => this.GetFloorPointPositions(index);
    private int[] get_floors_touching(int floor_index, float threshold) => this.GetIndicesTouching<Floor>(floor_index, threshold).ToArray();
    private Rect2 get_floor_bounding_box(int index) => this.GetBoundingBox<Floor>(index) ?? new();
    private Vector2 get_floor_centroid(int index) => this.GetCentroid<Floor>(index) ?? new();

    /* Other public methods */

    private Vector2 get_centroid() => this.GetCentroid();
    private Vector2 snap(Vector2 position, float threshold) => this.Snap(position, threshold);
    private Vector2 snap(Vector2 position) => this.Snap(position);
    private Vector2 snap_to_surface(Vector2 position, float threshold) => this.SnapToSurface(position, threshold);
    private Vector2 snap_to_surface(Vector2 position) => this.SnapToSurface(position);
    private CompoundMesh generate_mesh() => this.GenerateMesh();
}