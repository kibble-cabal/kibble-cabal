using Godot;

using MaterialMap = Godot.Collections.Dictionary<Godot.StringName, Godot.StringName>;

/// <summary>
/// Contains a reference to a particular wall on a building. Stores no data of it's own.
/// The sole purpose of this class is to make the API more streamlined without separating
/// data into multiple resources.
/// </summary>
[GlobalClass]
public partial class FloorRef(Building Building, int Index) : RefCounted
{
    public int index => Index;
    public Building building => Building;

    public Curve2D polygon
    {
        get => building.get_floor_polygon(index);
        set => building.set_floor_polygon(index, value);
    }

    public MaterialMap materials
    {
        get => building.get_floor_materials(index);
        set => building.set_floor_materials(index, value);
    }

    public StringName floor_id
    {
        get => building.get_floor_id(index);
        set => building.set_floor_id(index, value);
    }

    public int point_count => polygon.PointCount;

    public override string ToString() => $"Floor({polygon})";

    public Vector2[] tessellate(bool closed, int max_stages, float tolerance_degrees) => building.tessellate_floor(index, closed, max_stages, tolerance_degrees);
    public Vector2[] tessellate(bool closed, int max_stages) => building.tessellate_floor(index, closed, max_stages);
    public Vector2[] tessellate(bool closed) => building.tessellate_floor(index, closed);

    public bool is_valid() => building.is_floor_valid(index);
    public bool is_touching(int other, float threshold) => building.are_floors_touching(index, other, threshold);
    public bool is_touching(FloorRef other, float threshold) => building.are_floors_touching(index, other.index - 1, threshold);

    public void add_point(Vector2 position, Vector2 in_handle, Vector2 out_handle) => polygon.AddPoint(position, in_handle, out_handle);
    public void add_point(Vector2 position) => polygon.AddPoint(position);

    public void set_handles(int point_index, Vector2 in_handle, Vector2 out_handle)
    {
        polygon.SetPointIn(point_index, in_handle);
        polygon.SetPointOut(point_index, out_handle);
    }

    public Vector2 get_position(int point_index) => polygon.GetPointPosition(point_index);
    public Vector2 get_in_handle(int point_index) => polygon.GetPointIn(point_index);
    public Vector2 get_out_handle(int point_index) => polygon.GetPointOut(point_index);

    public Vector2[] get_point_positions() => building.get_floor_point_positions(index);

    public Vector2 snap(Vector2 position, float threshold) => building.snap_to_floor(index, position, threshold);
    public Vector2 snap(Vector2 position) => snap(position, -1);

    public Vector2 snap_to_surface(Vector2 position, float threshold) => building.snap_to_floor_surface(index, position, threshold);
    public Vector2 snap_to_surface(Vector2 position) => snap_to_surface(position, -1);
}