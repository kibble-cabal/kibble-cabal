using System.Linq;
using Godot;

using Godot.Collections;
using Collections = System.Collections.Generic;
using MaterialMap = Godot.Collections.Dictionary<Godot.StringName, Godot.StringName>;

#nullable enable

[GlobalClass]
public partial class Building : Resource
{
    /* Private properties */

    public Collections.List<Wall> Walls = [];
    public Collections.List<Floor> Floors = [];
    internal Callable ChangedCallable;

    Building()
    {
        this.ChangedCallable = Callable.From(EmitChanged);
    }

    /* Public properties */

    [Export]
    public Array wall_data
    {
        get => GetWallData();
        set => SetWallData(value);
    }

    public int wall_count => Walls.Count;

    [Export]
    public Array floor_data
    {
        get => GetFloorData();
        set => SetFloorData(value);
    }

    public int floor_count => Floors.Count;

    /* Private methods */

    private Array GetWallData() => Walls.Select(wall => (Variant)wall.Serialize()).ToGodotArray();

    private void SetWallData(Array value)
    {
        Walls.Clear();
        foreach (var val in value) Walls.Add(Wall.Deserialize(val.As<Array>()));
    }

    private Array GetFloorData() => Floors.Select(floor => (Variant)floor.Serialize()).ToGodotArray();

    private void SetFloorData(Array value)
    {
        Floors.Clear();
        foreach (var val in value) Floors.Add(Floor.Deserialize(val.As<Array>()));
        foreach (var floor in Floors) floor.Polygon.TryConnectChanged(ChangedCallable);
    }

    /* Public wall methods */

    public WallRef? get_wall(int index) => this.GetWallRef(index);
    public void remove_invalid_walls() => this.RemoveInvalidWalls();
    public bool has_wall(int index) => this.HasWall(index);
    public bool is_wall_valid(int index) => this.IsWallValid(index);
    public int add_wall() => this.AddWall(Vector2.Inf, Vector2.Inf);
    public int add_wall(Vector2 start, Vector2 end) => this.AddWall(start, end);
    public int add_wall(Vector2 start, Vector2 start_handle, Vector2 end, Vector2 end_handle) => this.AddWall(start, start_handle, end, end_handle);
    public Vector2 get_wall_start(int index) => this.GetWallStart(index);
    public Vector2 get_wall_end(int index) => this.GetWallEnd(index);
    public Vector2 get_wall_start_handle(int index) => this.GetWallStartHandle(index);
    public Vector2 get_wall_end_handle(int index) => this.GetWallEndHandle(index);
    public void set_wall_positions(int index, Vector2 start, Vector2 end) => this.SetWallPositions(index, start, end);
    public void set_wall_handles(int index, Vector2 start_handle, Vector2 end_handle) => this.SetWallHandles(index, start_handle, end_handle);
    public void set_wall(int index, Vector2 start, Vector2 start_handle, Vector2 end, Vector2 end_handle) => this.SetWall(index, start, start_handle, end, end_handle);
    public void set_wall_start(int index, Vector2 position) => this.SetWallStart(index, position);
    public void set_wall_end(int index, Vector2 position) => this.SetWallEnd(index, position);
    public void set_wall_start_handle(int index, Vector2 position) => this.SetWallStartHandle(index, position);
    public void set_wall_end_handle(int index, Vector2 position) => this.SetWallEndHandle(index, position);

    /* Public floor methods */

    public FloorRef? get_floor(int index) => this.GetFloorRef(index);
    public void remove_invalid_floors() => this.RemoveInvalidFloors();
    public bool has_floor(int index) => this.HasFloor(index);
    public bool is_floor_valid(int index) => this.IsFloorValid(index);
    public int add_floor() => this.AddFloor(new Curve2D());
    public int add_floor(Curve2D polygon) => this.AddFloor(polygon);

    public void set_floor_polygon(int index, Curve2D polygon)
    {
        if (has_floor(index))
        {
            Floors[index].Polygon = polygon;
            polygon.TryConnectChanged(ChangedCallable);
            EmitChanged();
        }
    }

    public void remove_wall(int index)
    {
        if (!has_wall(index)) return;
        Walls.RemoveAt(index);
        EmitChanged();
    }

    public void remove_floor(int index)
    {
        if (!has_floor(index)) return;
        Floors.RemoveAt(index);
        EmitChanged();
    }

    public MaterialMap get_wall_materials(int index) => this.GetWall(index)?.Materials ?? new MaterialMap();

    public StringName get_wall_material_id(int index, StringName material_name)
    {
        if (has_wall(index) && Walls[index].Materials.ContainsKey(material_name)) return Walls[index].Materials[material_name];
        return new StringName();
    }

    public StringName get_wall_interior_id(int index) => get_wall_material_id(index, "interior");
    public StringName get_wall_exterior_id(int index) => get_wall_material_id(index, "exterior");

    public MaterialMap get_floor_materials(int index) => this.GetFloor(index)?.Materials ?? new MaterialMap();
    public StringName get_floor_material_id(int index, StringName material_name)
    {
        if (has_floor(index) && Floors[index].Materials.ContainsKey(material_name)) return Floors[index].Materials[material_name];
        return new StringName();
    }

    public StringName get_floor_id(int index) => get_floor_material_id(index, "floor");

    public void set_wall_materials(int index, MaterialMap value)
    {
        if (has_wall(index)) Walls[index].Materials = value;
        EmitChanged();
    }

    public void set_wall_material_id(int index, StringName material_name, StringName id)
    {
        if (has_wall(index)) Walls[index].Materials[material_name] = id;
        EmitChanged();
    }

    public void set_wall_interior_id(int index, StringName id) => set_wall_material_id(index, "interior", id);
    public void set_wall_exterior_id(int index, StringName id) => set_wall_material_id(index, "exterior", id);

    public void set_floor_materials(int index, MaterialMap value)
    {
        if (has_floor(index)) Floors[index].Materials = value;
        EmitChanged();
    }

    public void set_floor_material_id(int index, StringName material_name, StringName id)
    {
        if (has_floor(index)) Floors[index].Materials[material_name] = id;
        EmitChanged();
    }

    public void set_floor_id(int index, StringName id) => set_floor_material_id(index, "floor", id);

    public Curve2D? get_floor_polygon(int index) => this.GetFloor(index)?.Polygon;

    public Vector2[] tessellate_wall(int index) => this.GetWall(index)?.Tessellate() ?? [];
    public Vector2[] tessellate_floor(int index, bool closed) => this.GetFloor(index)?.Tessellate(closed) ?? [];

    public Vector2 snap_to_wall(int index, Vector2 position, float threshold) => position.Snap(
        this.GetWall(index)?.Snap(position) ?? position,
        threshold
    );

    public Vector2 snap_to_wall(int index, Vector2 position) => snap_to_wall(index, position, -1);
    public Vector2 snap_to_wall_surface(int index, Vector2 position, float threshold) => position.Snap(
        this.GetWall(index)?.SnapToSurface(position) ?? position,
        threshold
    );
    public Vector2 snap_to_wall_surface(int index, Vector2 position) => snap_to_wall_surface(index, position, -1);
    /// <summary>
    /// Returns a new position, snapped to the nearest wall point, if the distance is below threshold.
    /// </summary>
    public Vector2 snap_to_walls(Vector2 position, float threshold)
    {
        var closestPoint = Vector2.Inf;
        foreach (var wall in Walls)
            closestPoint = position.Closest(closestPoint, wall.Snap(position));
        return position.Snap(closestPoint, threshold);
    }
    public Vector2 snap_to_walls_surface(Vector2 position, float threshold)
    {
        var closestPoint = Vector2.Inf;
        foreach (var wall in Walls)
            closestPoint = position.Closest(closestPoint, wall.SnapToSurface(position));
        return position.Snap(closestPoint, threshold);
    }
    public Vector2 snap_to_walls(Vector2 position) => snap_to_walls(position, -1);
    public Vector2 snap_to_walls_surface(Vector2 position) => snap_to_walls_surface(position, -1);

    public Vector2 snap_to_floor(int index, Vector2 position, float threshold) => position.Snap(
        this.GetFloor(index)?.Snap(position) ?? position,
        threshold
    );
    public Vector2 snap_to_floor(int index, Vector2 position) => snap_to_floor(index, position, -1);
    public Vector2 snap_to_floor_surface(int index, Vector2 position, float threshold) => position.Snap(
        this.GetFloor(index)?.SnapToSurface(position) ?? position,
        threshold
    );
    public Vector2 snap_to_floor_surface(int index, Vector2 position) => snap_to_floor_surface(index, position, -1);
    /// <summary>
    /// Returns a new position, snapped to the nearest floor point, if the distance is below threshold.
    /// </summary>
    public Vector2 snap_to_floors(Vector2 position, float threshold)
    {
        var closestPoint = Vector2.Inf;
        foreach (var floor in Floors)
            closestPoint = position.Closest(closestPoint, floor.Snap(position));
        return position.Snap(closestPoint, threshold);
    }
    public Vector2 snap_to_floors_surface(Vector2 position, float threshold)
    {
        var closestPoint = Vector2.Inf;
        foreach (var floor in Floors)
            closestPoint = position.Closest(closestPoint, floor.SnapToSurface(position));
        return position.Snap(closestPoint, threshold);
    }
    public Vector2 snap_to_floors(Vector2 position) => snap_to_floors(position, -1);
    public Vector2 snap_to_floors_surface(Vector2 position) => snap_to_floors_surface(position, -1);

    public Vector2 snap(Vector2 position, float threshold) => position.Snap(
        position.Closest(
            snap_to_floors(position),
            snap_to_walls(position)
        ),
        threshold
    );
    public Vector2 snap(Vector2 position) => snap(position, -1);
    public Vector2 snap_to_surface(Vector2 position, float threshold) => position.Snap(
        position.Closest(
            snap_to_walls_surface(position),
            snap_to_floors_surface(position)
        ),
        threshold
    );
    public Vector2 snap_to_surface(Vector2 position) => snap_to_surface(position, -1);

    public bool are_walls_touching(int a, int b) => this.GetWall(a)?.IsTouching(this.GetWall(b)) ?? false;
    public bool are_floors_touching(int a, int b, float threshold) => this.GetFloor(a)?.IsTouching(this.GetFloor(b), threshold) ?? false;

    public Vector2[] get_floor_point_positions(int index) => this.GetFloor(index)?.GetPointPositions() ?? [];

    public CompoundMesh generate_mesh()
    {
        var mesh = new CompoundMesh()
        {
            meshes = [
                ..Walls.Where(wall => wall.IsValid()).SelectMany(wall => wall.GenerateMeshes(this)),
                ..Floors.Where(floor => floor.IsValid()).Select(floor => floor.GenerateMesh(Wall.TessellationStages, Wall.TessellationToleranceDegrees))
            ]
        };
        // Connect("changed", new Callable(mesh, "generate"));
        mesh.generate();
        return mesh;
    }

    public int[] get_walls_touching(int wall_index)
    {
        Wall? currentWall = this.GetWall(wall_index);
        if (currentWall == null) return [];
        return Walls.Select((wall, index) => wall.IsTouching(currentWall) ? index : -1).Where(index => index != -1 && index != wall_index).ToArray();
    }

    public int[] get_floors_touching(int floor_index, float threshold)
    {
        Floor? currentFloor = this.GetFloor(floor_index);
        if (currentFloor == null) return [];
        return Floors.Select((floor, index) => currentFloor.IsTouching(floor, threshold) ? index : -1).Where(index => index != -1).ToArray();
    }
}