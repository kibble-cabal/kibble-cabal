using Godot;

using Godot.Collections;
using Collections = System.Collections.Generic;
using MaterialMap = Godot.Collections.Dictionary<Godot.StringName, Godot.StringName>;


[GlobalClass]
public partial class Building : Resource
{
    /* Private properties */

    private Collections.List<Wall> Walls = [];
    private Collections.List<Floor> Floors = [];
    private Callable ChangedCallable;

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

    private Array GetWallData()
    {
        Array data = [];
        foreach (var wall in Walls)
            data.Add(wall.ToData());
        return data;
    }

    private void SetWallData(Array value)
    {
        Walls.Clear();
        for (var i = 0; i < value.Count; i++)
            Walls.Add(Wall.FromData(value[i].As<Array>()));
    }

    private Array GetFloorData()
    {
        Array data = [];
        foreach (var floor in Floors)
            data.Add(floor.ToData());
        return data;
    }

    private void SetFloorData(Array value)
    {
        Floors.Clear();
        for (var i = 0; i < value.Count; i++)
            Floors.Add(Floor.FromData(value[i].As<Array>()));
        foreach (var floor in Floors)
            floor.Polygon.Connect("changed", ChangedCallable);
    }

    private Wall GetWall(int index) => has_wall(index) ? Walls[index] : null;
    private Floor GetFloor(int index) => has_floor(index) ? Floors[index] : null;

    private void TryConnectChanged(Resource resource)
    {
        if (!resource.IsConnected("changed", ChangedCallable))
            resource.Connect("changed", ChangedCallable);
    }

    /* Public methods */

    public WallRef get_wall(int index) => new(this, index);
    public FloorRef get_floor(int index) => new(this, index);

    /// <summary>
    /// Removes all invalid walls from this building. See <see cref="Wall.IsValid"/> 
    /// </summary>
    public void remove_invalid_walls()
    {
        for (var i = Walls.Count - 1; i >= 0; i--)
            if (!Walls[i].IsValid()) remove_wall(i);
    }

    public void remove_invalid_floors()
    {
        for (var i = Floors.Count - 1; i >= 0; i--)
            if (!Floors[i].IsValid()) remove_floor(i);
    }

    public bool has_wall(int index) => index >= 0 && Walls.Count > index;
    public bool has_floor(int index) => index >= 0 && Floors.Count > index;
    public bool is_wall_valid(int index) => GetWall(index)?.IsValid() ?? false;
    public bool is_floor_valid(int index) => GetFloor(index)?.IsValid() ?? false;

    public int add_wall() => add_wall(Vector2.Inf, Vector2.Inf);
    public int add_wall(Vector2 a_position, Vector2 b_position)
    {
        Walls.Add(new Wall(a_position, b_position));
        EmitChanged();
        return Walls.Count - 1;
    }

    public int add_floor() => add_floor(new Curve2D());
    public int add_floor(Curve2D polygon)
    {
        Floors.Add(new Floor(polygon));
        TryConnectChanged(polygon);
        EmitChanged();
        return Floors.Count - 1;
    }

    public void set_wall_positions(int index, Vector2 a_position, Vector2 b_position)
    {
        set_wall_start(index, a_position);
        set_wall_end(index, b_position);
    }

    public void set_wall_handles(int index, Vector2 start_handle, Vector2 end_handle)
    {
        set_wall_start_handle(index, start_handle);
        set_wall_end_handle(index, end_handle);
    }

    public void set_wall(int index, Vector2 a_position, Vector2 start_handle, Vector2 b_position, Vector2 end_handle)
    {
        set_wall_positions(index, a_position, b_position);
        set_wall_handles(index, start_handle, end_handle);
    }

    public void set_wall_start(int index, Vector2 position)
    {
        if (has_wall(index)) Walls[index].Start = position;
        EmitChanged();
    }

    public void set_wall_end(int index, Vector2 position)
    {
        if (has_wall(index)) Walls[index].End = position;
        EmitChanged();
    }

    public void set_wall_start_handle(int index, Vector2 position)
    {
        if (has_wall(index)) Walls[index].StartHandle = position;
        EmitChanged();
    }

    public void set_wall_end_handle(int index, Vector2 position)
    {
        if (has_wall(index)) Walls[index].EndHandle = position;
        EmitChanged();
    }

    public void set_floor_polygon(int index, Curve2D polygon)
    {
        if (has_floor(index))
        {
            Floors[index].Polygon = polygon;
            TryConnectChanged(polygon);
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

    public MaterialMap get_wall_materials(int index) => GetWall(index)?.Materials ?? new MaterialMap();

    public StringName get_wall_material_id(int index, StringName material_name)
    {
        if (has_wall(index) && Walls[index].Materials.ContainsKey(material_name)) return Walls[index].Materials[material_name];
        return new StringName();
    }

    public StringName get_wall_interior_id(int index) => get_wall_material_id(index, "interior");
    public StringName get_wall_exterior_id(int index) => get_wall_material_id(index, "exterior");

    public MaterialMap get_floor_materials(int index) => GetFloor(index)?.Materials ?? new MaterialMap();
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

    public Vector2 get_wall_start(int index) => GetWall(index)?.Start ?? Vector2.Inf;
    public Vector2 get_wall_end(int index) => GetWall(index)?.End ?? Vector2.Inf;
    public Vector2 get_wall_start_handle(int index) => GetWall(index)?.StartHandle ?? Vector2.Inf;
    public Vector2 get_wall_end_handle(int index) => GetWall(index)?.EndHandle ?? Vector2.Inf;

    public Curve2D get_floor_polygon(int index) => GetFloor(index)?.Polygon;

    public Vector2[] tessellate_wall(int index) => tessellate_wall(index, 5, 4);
    public Vector2[] tessellate_wall(int index, int max_stages) => tessellate_wall(index, max_stages, 4);
    public Vector2[] tessellate_wall(int index, int max_stages, float tolerance_degrees) => GetWall(index)?.Tessellate(max_stages, tolerance_degrees) ?? [];

    public Vector2[] tessellate_floor(int index, bool closed) => tessellate_floor(index, closed, 5, 4);
    public Vector2[] tessellate_floor(int index, bool closed, int max_stages) => tessellate_floor(index, closed, max_stages, 4);
    public Vector2[] tessellate_floor(int index, bool closed, int max_stages, float tolerance_degrees) => GetFloor(index)?.Tessellate(closed, max_stages, tolerance_degrees) ?? [];

    public Vector2 snap_to_wall(int index, Vector2 position, float threshold) => position.Snap(
        GetWall(index)?.Snap(position) ?? position,
        threshold
    );
    public Vector2 snap_to_wall(int index, Vector2 position) => snap_to_wall(index, position, -1);
    public Vector2 snap_to_wall_surface(int index, Vector2 position, float threshold) => position.Snap(
        GetWall(index)?.SnapToSurface(position) ?? position,
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
        GetFloor(index)?.Snap(position) ?? position,
        threshold
    );
    public Vector2 snap_to_floor(int index, Vector2 position) => snap_to_floor(index, position, -1);
    public Vector2 snap_to_floor_surface(int index, Vector2 position, float threshold) => position.Snap(
        GetFloor(index)?.SnapToSurface(position) ?? position,
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

    public bool are_walls_touching(int a, int b, float threshold) => has_wall(a) && has_wall(b) ? GetWall(a).IsTouching(GetWall(b), threshold) : false;
    public bool are_floors_touching(int a, int b, float threshold) => has_floor(a) && has_floor(b) ? GetFloor(a).IsTouching(GetFloor(b), threshold) : false;

    public Vector2[] get_floor_point_positions(int index) => GetFloor(index)?.GetPointPositions() ?? [];
}