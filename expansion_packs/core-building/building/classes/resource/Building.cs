using System.Linq;
using Godot;

using Godot.Collections;
using Collections = System.Collections.Generic;
using MaterialMap = Godot.Collections.Dictionary<Godot.StringName, Godot.StringName>;

#nullable enable

[GlobalClass]
public partial class Building : Resource
{
    public const int TessellationStages = 3;
    public const float TessellationToleranceDegrees = 3;

    /* Private properties */

    public Collections.List<Wall> Walls = [];
    public Collections.List<Floor> Floors = [];
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
            foreach (var val in value) Walls.Add(Wall.Deserialize(val.As<Array>()));
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
            foreach (var val in value) Floors.Add(Floor.Deserialize(val.As<Array>()));
            foreach (var floor in Floors) floor.Polygon.TryConnectChanged(ChangedCallable);
        }
    }

    public int floor_count => Floors.Count;

    /* Public wall methods */

    private WallRef? get_wall(int index) => this.GetWallRef(index);
    private void remove_invalid_walls() => this.RemoveInvalidWalls();
    private bool has_wall(int index) => this.HasWall(index);
    private bool is_wall_valid(int index) => this.IsWallValid(index);
    private int add_wall() => this.AddWall(Vector2.Inf, Vector2.Inf);
    private int add_wall(Vector2 start, Vector2 end) => this.AddWall(start, end);
    private int add_wall(Vector2 start, Vector2 start_handle, Vector2 end, Vector2 end_handle) => this.AddWall(start, start_handle, end, end_handle);
    private Vector2 get_wall_start(int index) => this.GetWallStart(index);
    private Vector2 get_wall_end(int index) => this.GetWallEnd(index);
    private Vector2 get_wall_start_handle(int index) => this.GetWallStartHandle(index);
    private Vector2 get_wall_end_handle(int index) => this.GetWallEndHandle(index);
    private void set_wall_positions(int index, Vector2 start, Vector2 end) => this.SetWallPositions(index, start, end);
    private void set_wall_handles(int index, Vector2 start_handle, Vector2 end_handle) => this.SetWallHandles(index, start_handle, end_handle);
    private void set_wall(int index, Vector2 start, Vector2 start_handle, Vector2 end, Vector2 end_handle) => this.SetWall(index, start, start_handle, end, end_handle);
    private void set_wall_start(int index, Vector2 position) => this.SetWallStart(index, position);
    private void set_wall_end(int index, Vector2 position) => this.SetWallEnd(index, position);
    private void set_wall_start_handle(int index, Vector2 position) => this.SetWallStartHandle(index, position);
    private void set_wall_end_handle(int index, Vector2 position) => this.SetWallEndHandle(index, position);
    private void remove_wall(int index) => this.RemoveWall(index);
    private MaterialMap get_wall_materials(int index) => this.GetWallMaterials(index);
    private StringName get_wall_material_id(int index, StringName material_name) => this.GetWallMaterialID(index, material_name) ?? new();
    private StringName get_wall_interior_id(int index) => this.GetWallInteriorID(index) ?? new();
    private StringName get_wall_exterior_id(int index) => this.GetWallExteriorID(index) ?? new();
    private void set_wall_materials(int index, MaterialMap value) => this.SetWallMaterials(index, value);
    private void set_wall_material_id(int index, StringName material_name, StringName id) => this.SetWallMaterialID(index, material_name, id);
    private void set_wall_interior_id(int index, StringName id) => this.SetWallInteriorID(index, id);
    private void set_wall_exterior_id(int index, StringName id) => this.SetWallExteriorID(index, id);
    private Vector2[] tessellate_wall(int index) => this.TessellateWall(index);
    private Vector2 snap_to_wall(int index, Vector2 position, float threshold) => this.SnapToWall(index, position, threshold);
    private Vector2 snap_to_wall(int index, Vector2 position) => this.SnapToWall(index, position);
    private Vector2 snap_to_wall_surface(int index, Vector2 position, float threshold) => this.SnapToWallSurface(index, position, threshold);
    private Vector2 snap_to_wall_surface(int index, Vector2 position) => this.SnapToWallSurface(index, position);
    private Vector2 snap_to_walls(Vector2 position, float threshold) => this.SnapToWalls(position, threshold);
    private Vector2 snap_to_walls(Vector2 position) => this.SnapToWalls(position);
    private Vector2 snap_to_walls_surface(Vector2 position, float threshold) => this.SnapToWallsSurface(position, threshold);
    private Vector2 snap_to_walls_surface(Vector2 position) => this.SnapToWallsSurface(position);
    private bool are_walls_touching(int a, int b) => this.AreWallsTouching(a, b);
    private int[] get_walls_touching(int wall_index) => this.GetWallIndicesTouching(wall_index).ToArray();

    /* Public floor methods */

    private FloorRef? get_floor(int index) => this.GetFloorRef(index);
    private void remove_invalid_floors() => this.RemoveInvalidFloors();
    private bool has_floor(int index) => this.HasFloor(index);
    private bool is_floor_valid(int index) => this.IsFloorValid(index);
    private int add_floor() => this.AddFloor(new Curve2D());
    private int add_floor(Curve2D polygon) => this.AddFloor(polygon);
    private void set_floor_polygon(int index, Curve2D polygon) => this.SetFloorPolygon(index, polygon);
    private void remove_floor(int index) => this.RemoveFloor(index);
    private MaterialMap get_floor_materials(int index) => this.GetFloorMaterials(index);
    private StringName get_floor_material_id(int index, StringName material_name) => this.GetFloorMaterialID(index, material_name);
    private StringName get_floor_id(int index) => this.GetFloorID(index);
    private void set_floor_materials(int index, MaterialMap value) => this.SetFloorMaterials(index, value);
    private void set_floor_material_id(int index, StringName material_name, StringName id) => this.SetFloorMaterialID(index, material_name, id);
    private void set_floor_id(int index, StringName id) => this.SetFloorID(index, id);
    private Curve2D? get_floor_polygon(int index) => this.GetFloorPolygon(index);
    private Vector2[] tessellate_floor(int index) => this.TessellateFloor(index);
    private Vector2 snap_to_floor(int index, Vector2 position, float threshold) => this.SnapToFloor(index, position, threshold);
    private Vector2 snap_to_floor(int index, Vector2 position) => this.SnapToFloor(index, position);
    private Vector2 snap_to_floor_surface(int index, Vector2 position, float threshold) => this.SnapToFloorSurface(index, position, threshold);
    private Vector2 snap_to_floor_surface(int index, Vector2 position) => this.SnapToFloorSurface(index, position, -1);
    private Vector2 snap_to_floors(Vector2 position, float threshold) => this.SnapToFloors(position, threshold);
    private Vector2 snap_to_floors(Vector2 position) => this.SnapToFloors(position);
    private Vector2 snap_to_floors_surface(Vector2 position, float threshold) => this.SnapToFloorsSurface(position, threshold);
    private Vector2 snap_to_floors_surface(Vector2 position) => this.SnapToFloorsSurface(position);
    private bool are_floors_touching(int a, int b, float threshold) => this.AreFloorsTouching(a, b, threshold);
    private Vector2[] get_floor_point_positions(int index) => this.GetFloorPointPositions(index);
    private int[] get_floors_touching(int floor_index, float threshold) => this.GetFloorsTouching(floor_index, threshold);

    /* Other public methods */

    private Vector2 snap(Vector2 position, float threshold) => this.Snap(position, threshold);
    private Vector2 snap(Vector2 position) => this.Snap(position);
    private Vector2 snap_to_surface(Vector2 position, float threshold) => this.SnapToSurface(position, threshold);
    private Vector2 snap_to_surface(Vector2 position) => this.SnapToSurface(position);
    private CompoundMesh generate_mesh() => this.GenerateMesh();
}