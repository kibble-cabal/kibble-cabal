using Godot;
using Godot.Collections;

/// <summary>
/// Contains a reference to a particular wall on a building. Stores no data of its own.
/// The sole purpose of this class is to make the API more streamlined without separating
/// data into multiple resources.
/// </summary>
[GlobalClass]
public partial class WallRef(Building Building, int Index) : RefCounted
{
    internal Building building => Building;
    internal int index => Index;

    private Vector2 start
    {
        get => building.GetWallStart(index);
        set => building.SetWallStart(index, value);
    }

    private Vector2 end
    {
        get => building.GetWallEnd(index);
        set => building.SetWallEnd(index, value);
    }

    private Vector2 start_handle
    {
        get => building.GetWallStartHandle(index);
        set => building.SetWallStartHandle(index, value);
    }

    private Vector2 end_handle
    {
        get => building.GetWallEndHandle(index);
        set => building.SetWallEndHandle(index, value);
    }

    private Dictionary<StringName, StringName> materials
    {
        get => building.GetMaterials<Wall>(index);
        set => building.SetMaterials<Wall>(index, value);
    }

    private StringName interior_id
    {
        get => building.GetWallInteriorID(index);
        set => building.SetWallInteriorID(index, value);
    }

    private StringName exterior_id
    {
        get => building.GetWallExteriorID(index);
        set => building.SetWallExteriorID(index, value);
    }

    private float thickness
    {
        get => building.GetWallThickness(index);
        set => building.SetWallThickness(index, value);
    }

    private float height
    {
        get => building.GetWallHeight(index);
        set => building.SetWallHeight(index, value);
    }

    public override string ToString()
    {
        if (start_handle.IsZeroApprox() && end_handle.IsZeroApprox()) return $"Wall {{ start: {start}, end: {end} }}";
        return $"Wall {{ start: {start}, end: {end}, start_handle: {start_handle}, end_handle: {end_handle} }}";
    }

    private Vector2 get_midpoint() => building.GetWallMidpoint(index);
    private Vector2[] tessellate() => building.Tessellate<Wall>(index);
    private bool is_valid() => building.IsValid<Wall>(index);
    private bool has_start() => start.IsFinite();
    private bool has_end() => end.IsFinite();
    private bool is_touching(int other) => building.IsTouching<Wall>(index, other);
    private Vector2 snap(Vector2 position, float threshold) => building.Snap<Wall>(index, position, threshold);
    private Vector2 snap(Vector2 position) => snap(position, -1);
    private Vector2 snap_to_surface(Vector2 position, float threshold) => building.SnapToSurface<Wall>(index, position, threshold);
    private Vector2 snap_to_surface(Vector2 position) => snap_to_surface(position, -1);
}