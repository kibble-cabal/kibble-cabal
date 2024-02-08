using Godot;

using MaterialMap = Godot.Collections.Dictionary<Godot.StringName, Godot.StringName>;

/// <summary>
/// Contains a reference to a particular wall on a building. Stores no data of it's own.
/// The sole purpose of this class is to make the API more streamlined without separating
/// data into multiple resources.
/// </summary>
[GlobalClass]
public partial class WallRef(Building Building, int Index) : RefCounted
{
    public Building building => Building;
    public int index => Index;

    public Vector2 start
    {
        get => building.get_wall_start(index);
        set => building.set_wall_start(index, value);
    }

    public Vector2 end
    {
        get => building.get_wall_end(index);
        set => building.set_wall_end(index, value);
    }

    public Vector2 start_handle
    {
        get => building.get_wall_start_handle(index);
        set => building.set_wall_start_handle(index, value);
    }

    public Vector2 end_handle
    {
        get => building.get_wall_end_handle(index);
        set => building.set_wall_end_handle(index, value);
    }

    public MaterialMap materials
    {
        get => building.get_wall_materials(index);
        set => building.set_wall_materials(index, value);
    }

    public StringName interior_id
    {
        get => building.get_wall_interior_id(index);
        set => building.set_wall_interior_id(index, value);
    }

    public StringName exterior_id
    {
        get => building.get_wall_exterior_id(index);
        set => building.set_wall_exterior_id(index, value);
    }

    public override string ToString()
    {
        if (start_handle.IsFinite() || end_handle.IsFinite())
            return $"Wall[start: {start}, end: {end}, start_handle: {start_handle}, end_handle: {end_handle}]";
        return $"Wall[start: {start}, end: {end}]";
    }

    public Vector2[] tessellate(int max_stages, float tolerance_degrees) => building.tessellate_wall(index, max_stages, tolerance_degrees);
    public Vector2[] tessellate(int max_stages) => building.tessellate_wall(index, max_stages);
    public Vector2[] tessellate() => building.tessellate_wall(index);

    public bool is_valid() => building.is_wall_valid(index);
    public bool has_start() => start.IsFinite();
    public bool has_end() => end.IsFinite();

    public bool is_touching(int other, float threshold) => building.are_walls_touching(index, other, threshold);
    public bool is_touching(WallRef other, float threshold) => building.are_walls_touching(index, other.index, threshold);
}