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
    internal int index => Index;
    internal Building building => Building;

    private Curve2D polygon
    {
        get => building.GetFloorPolygon(index);
        set => building.SetFloorPolygon(index, value);
    }

    private MaterialMap materials
    {
        get => building.GetFloorMaterials(index);
        set => building.SetFloorMaterials(index, value);
    }

    private StringName floor_id
    {
        get => building.GetFloorID(index);
        set => building.SetFloorID(index, value);
    }

    private int point_count => polygon.PointCount;

    public override string ToString() => $"Floor({polygon})";

    private Vector2[] tessellate() => building.TessellateFloor(index);

    private bool is_valid() => building.IsFloorValid(index);
    private bool is_touching(int other, float threshold) => building.AreFloorsTouching(index, other, threshold);

    private void add_point(Vector2 position, Vector2 in_handle, Vector2 out_handle) => polygon.AddPoint(position, in_handle, out_handle);
    private void add_point(Vector2 position) => polygon.AddPoint(position);

    private void set_handles(int point_index, Vector2 in_handle, Vector2 out_handle)
    {
        polygon.SetPointIn(point_index, in_handle);
        polygon.SetPointOut(point_index, out_handle);
    }

    private Vector2 get_position(int point_index) => polygon.GetPointPosition(point_index);
    private Vector2 get_in_handle(int point_index) => polygon.GetPointIn(point_index);
    private Vector2 get_out_handle(int point_index) => polygon.GetPointOut(point_index);

    private Vector2[] get_point_positions() => building.GetFloorPointPositions(index);

    private Vector2 snap(Vector2 position, float threshold) => building.SnapToFloor(index, position, threshold);
    private Vector2 snap(Vector2 position) => snap(position, -1);

    private Vector2 snap_to_surface(Vector2 position, float threshold) => building.SnapToFloorSurface(index, position, threshold);
    private Vector2 snap_to_surface(Vector2 position) => snap_to_surface(position, -1);
}