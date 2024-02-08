using Godot;

using MaterialMap = Godot.Collections.Dictionary<Godot.StringName, Godot.StringName>;

/// <summary>
/// Contains a reference to a particular wall on a building. Stores no data of it's own.
/// The sole purpose of this class is to make the API more streamlined without separating
/// data into multiple resources.
/// </summary>
[GlobalClass]
public partial class FloorRef(Building building, int index) : RefCounted
{
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

    public Vector2[] tessellate(int max_stages, float tolerance_degrees) => polygon.Tessellate(max_stages, tolerance_degrees);
    public Vector2[] tessellate(int max_stages) => polygon.Tessellate(max_stages);
    public Vector2[] tessellate() => polygon.Tessellate();

    public bool is_valid() => polygon.PointCount > 2;
}