using System.Linq;
using Godot;

#nullable enable

/// <summary>
/// This class contains functions related to building walls that should not be exposed directly to GDScript.
/// </summary>
internal static class BuildingWallExtensions
{
    static internal Wall? GetWall(this Building building, int index) => building.Walls.ElementAtOrDefault(index);

    static internal WallRef? GetWallRef(this Building building, int index) => building.HasWall(index) ? new(building, index) : null;

    static internal bool HasWall(this Building building, int index) => index >= 0 && index < building.Walls.Count;

    static internal int AddWall(this Building building, Vector2 start, Vector2 startHandle, Vector2 end, Vector2 endHandle)
    {
        building.Walls.Add(new Wall(start, startHandle, end, endHandle));
        building.EmitChanged();
        return building.Walls.Count - 1;
    }

    static internal int AddWall(this Building building, Vector2 start, Vector2 end) => building.AddWall(start, Vector2.Zero, end, Vector2.Zero);

    /// <summary>
    /// Removes all invalid walls from this building. See <see cref="Wall.IsValid"/> 
    /// </summary>
    static internal void RemoveInvalidWalls(this Building building) => building.Walls.RemoveAll(wall => !wall.IsValid());

    static internal bool IsWallValid(this Building building, int index) => building.GetWall(index)?.IsValid() ?? false;

    static internal void SetWallPositions(this Building building, int index, Vector2 start, Vector2 end)
    {
        building.SetWallStart(index, start);
        building.SetWallEnd(index, end);
    }

    static internal void SetWallHandles(this Building building, int index, Vector2 startHandle, Vector2 endHandle)
    {
        building.SetWallStartHandle(index, startHandle);
        building.SetWallEndHandle(index, endHandle);
    }

    static internal void SetWall(this Building building, int index, Vector2 start, Vector2 startHandle, Vector2 end, Vector2 endHandle)
    {
        building.SetWallPositions(index, start, end);
        building.SetWallHandles(index, startHandle, endHandle);
    }

    static internal Vector2 GetWallStart(this Building building, int index) => building.GetWall(index)?.Start ?? Vector2.Inf;
    static internal void SetWallStart(this Building building, int index, Vector2 position)
    {
        if (building.GetWall(index) is Wall wall) wall.Start = position;
        building.EmitChanged();
    }

    static internal Vector2 GetWallEnd(this Building building, int index) => building.GetWall(index)?.End ?? Vector2.Inf;
    static internal void SetWallEnd(this Building building, int index, Vector2 position)
    {
        if (building.GetWall(index) is Wall wall) wall.End = position;
        building.EmitChanged();
    }

    static internal Vector2 GetWallStartHandle(this Building building, int index) => building.GetWall(index)?.StartHandle ?? Vector2.Inf;
    static internal void SetWallStartHandle(this Building building, int index, Vector2 position)
    {
        if (building.GetWall(index) is Wall wall) wall.StartHandle = position;
        building.EmitChanged();
    }

    static internal Vector2 GetWallEndHandle(this Building building, int index) => building.GetWall(index)?.EndHandle ?? Vector2.Inf;
    static internal void SetWallEndHandle(this Building building, int index, Vector2 position)
    {
        if (building.GetWall(index) is Wall wall) wall.EndHandle = position;
        building.EmitChanged();
    }

}

/// <summary>
/// This class contains functions related to building floors that should not be exposed directly to GDScript.
/// </summary>
internal static class BuildingFloorExtensions
{
    static internal Floor? GetFloor(this Building building, int index) => building.Floors.ElementAtOrDefault(index);

    static internal FloorRef? GetFloorRef(this Building building, int index) => building.HasFloor(index) ? new(building, index) : null;

    static internal bool HasFloor(this Building building, int index) => index >= 0 && index < building.Floors.Count;

    /// <summary>
    /// Removes all invalid floors from this building. See <see cref="Floor.IsValid"/> 
    /// </summary>
    static internal void RemoveInvalidFloors(this Building building) => building.Floors.RemoveAll(floor => !floor.IsValid());

    static internal bool IsFloorValid(this Building building, int index) => building.GetWall(index)?.IsValid() ?? false;

    static internal int AddFloor(this Building building, Curve2D polygon)
    {
        building.Floors.Add(new Floor(polygon));
        polygon.TryConnectChanged(building.ChangedCallable);
        building.EmitChanged();
        return building.Floors.Count - 1;
    }
}