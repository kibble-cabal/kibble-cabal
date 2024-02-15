using System;
using System.Collections.Generic;
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

    static internal int[] GetAllConnectedWalls(this Building building, int index)
    {
        int[] connectedWalls = [], addedWalls = [index];
        while (addedWalls.Length > 0)
        {
            var adding = addedWalls
                .Except(connectedWalls)
                .SelectMany(i => building.GetWallIndicesTouching(i).Except(connectedWalls).Except(addedWalls))
                .ToArray();
            connectedWalls = [.. connectedWalls, .. addedWalls];
            addedWalls = adding;
        }
        return connectedWalls;
    }

    static internal void SelectConnectedWalls(this Building building, int index, Action<Wall> predicate) => building.SelectConnectedWalls(index, wall => predicate(wall));
    static internal void SelectConnectedWalls(this Building building, int index, Action<Wall, int> predicate) => building.GetAllConnectedWalls(index).ForEach(i => predicate(building.GetWall(i)!, i));
    static internal T[] SelectConnectedWalls<T>(this Building building, int index, Func<Wall, T> predicate) => building.SelectConnectedWalls(index, (wall, _) => predicate(wall));
    static internal T[] SelectConnectedWalls<T>(this Building building, int index, Func<Wall, int, T> predicate) => building.GetAllConnectedWalls(index).Select(i => predicate(building.GetWall(i)!, i)).ToArray();

    static internal bool HasWall(this Building building, int index) => index >= 0 && index < building.Walls.Count;

    static internal int AddWall(this Building building, Vector2 start, Vector2 startHandle, Vector2 end, Vector2 endHandle)
    {
        int index = building.Walls.Count;
        building.Walls.Add(new Wall(start, startHandle, end, endHandle));
        building.EmitChanged();
        building.EmitSignal(nameof(building.WallAdded), [index]);
        return index;
    }

    static internal int AddWall(this Building building, Godot.Collections.Array data) => Wall.Deserialize(data).Match(
        ok: wall =>
        {
            int index = building.Walls.Count;
            building.Walls.Add(wall);
            building.EmitChanged();
            building.EmitSignal(nameof(building.WallAdded), [index]);
            return index;
        },
        error: _ =>
        {
            GD.PushError($"Unable to deserialize array data into Wall: {data}. Returning invalid index.");
            return -1;
        }
    );

    static internal int AddWall(this Building building, Vector2 start, Vector2 end) => building.AddWall(start, Vector2.Zero, end, Vector2.Zero);

    static internal void AddWalls(this Building building, Vector2[] points)
    {
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2 start = points[i], end = points[i + 1];
            building.AddWall(start, end);
        }
    }

    static internal void AddWalls(this Building building, Curve2D curve)
    {
        for (int i = 0; i < curve.PointCount - 1; i++)
        {
            Vector2 start = curve.GetPointPosition(i), end = curve.GetPointPosition(i + 1);
            Vector2 startHandle = curve.GetPointOut(i), endHandle = curve.GetPointIn(i + 1);
            building.AddWall(start, startHandle, end, endHandle);
        }
    }

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

    static internal float GetWallHeight(this Building building, int index) => building.GetWall(index)?.Height ?? Wall.DefaultHeight;
    static internal void SetWallHeight(this Building building, int index, float value)
    {
        if (building.GetWall(index) is Wall wall) wall.Height = value;
        building.EmitChanged();
    }

    static internal float GetWallThickness(this Building building, int index) => building.GetWall(index)?.Thickness ?? Wall.DefaultThickness;
    static internal void SetWallThickness(this Building building, int index, float value)
    {
        if (building.GetWall(index) is Wall wall) wall.Thickness = value;
        building.EmitChanged();
    }

    /// <summary>
    /// Modifies all connected walls' heights.
    /// </summary>
    static internal void FillWallHeight(this Building building, int index, float value)
    {
        building.SelectConnectedWalls(index, wall => wall.Height = value);
        building.EmitChanged();
    }

    /// <summary>
    /// Modifies all connected walls' thickness.
    /// </summary>
    static internal void FillWallThickness(this Building building, int index, float value)
    {
        building.SelectConnectedWalls(index, wall => wall.Thickness = value);
        building.EmitChanged();
    }

    static internal void RemoveWall(this Building building, int index)
    {
        if (!building.HasWall(index)) return;
        var data = building.Walls[index].Serialize();
        building.Walls.RemoveAt(index);
        building.EmitChanged();
        building.EmitSignal(nameof(building.WallRemoved), [index, data]);
    }

    static internal void RemoveConnectedWalls(this Building building, int index)
    {
        building.GetAllConnectedWalls(index).Distinct().OrderByDescending(i => i).ForEach(building.Walls.RemoveAt);
        building.EmitChanged();
    }

    static internal void MoveConnectedWallsBy(this Building building, int index, Vector2 delta)
    {
        building.SelectConnectedWalls(index, wall => wall.MoveBy(delta));
        building.EmitChanged();
    }

    static internal MaterialMap GetWallMaterials(this Building building, int index) => building.GetWall(index)?.Materials ?? new MaterialMap();
    static internal void SetWallMaterials(this Building building, int index, MaterialMap value)
    {
        if (building.HasWall(index)) building.Walls[index].Materials = value;
        building.EmitChanged();
    }

    static internal StringName? GetWallMaterialID(this Building building, int index, StringName materialName)
    {
        if (building.GetWall(index)?.Materials.ContainsKey(materialName) ?? false) return building.Walls[index].Materials[materialName];
        return null;
    }
    static internal void SetWallMaterialID(this Building building, int index, StringName materialName, StringName id)
    {
        if (building.HasWall(index)) building.Walls[index].Materials.Add(materialName, id);
        building.EmitChanged();
    }

    static internal StringName? GetWallInteriorID(this Building building, int index) => building.GetWallMaterialID(index, "interior");
    static internal void SetWallInteriorID(this Building building, int index, StringName id) => building.SetWallMaterialID(index, "interior", id);

    static internal StringName? GetWallExteriorID(this Building building, int index) => building.GetWallMaterialID(index, "exterior");
    static internal void SetWallExteriorID(this Building building, int index, StringName id) => building.SetWallMaterialID(index, "exterior", id);

    static internal Vector2 GetWallMidpoint(this Building building, int index) => building.GetWall(index)?.GetMidpoint() ?? Vector2.Inf;

    static internal Vector2[] TessellateWall(this Building building, int index) => building.GetWall(index)?.Tessellate() ?? [];

    static internal Vector2 SnapToWall(this Building building, int index, Vector2 position, float threshold = -1) => position.Snap(
        building.GetWall(index)?.Snap(position) ?? position,
        threshold
    );

    static internal Vector2 SnapToWallSurface(this Building building, int index, Vector2 position, float threshold = -1) => position.Snap(
        building.GetWall(index)?.SnapToSurface(position) ?? position,
        threshold
    );

    /// <summary>
    /// Returns a new position, snapped to the nearest wall point, if the distance is below threshold.
    /// </summary>
    static internal Vector2 SnapToWalls(this Building building, Vector2 position, float threshold = -1)
    {
        var closestPoint = Vector2.Inf;
        foreach (var wall in building.Walls)
            closestPoint = position.Closest(closestPoint, wall.Snap(position));
        return position.Snap(closestPoint, threshold);
    }

    static internal Vector2 SnapToWallsSurface(this Building building, Vector2 position, float threshold = -1)
    {
        var closestPoint = Vector2.Inf;
        foreach (var wall in building.Walls)
            closestPoint = position.Closest(closestPoint, wall.SnapToSurface(position));
        return position.Snap(closestPoint, threshold);
    }

    static internal bool AreWallsTouching(this Building building, int indexA, int indexB) => building.GetWall(indexA)?.IsTouching(building.GetWall(indexB)) ?? false;

    static internal IEnumerable<Wall> GetWallsTouching(this Building building, Wall? wall) => wall == null ? [] : building.Walls
        .Where(currentWall => currentWall != wall && currentWall.IsValid())
        .Select(currentWall => wall.IsTouching(currentWall) ? currentWall : null)
        .WhereNotNull();

    static internal IEnumerable<int> GetWallIndicesTouching(this Building building, Wall? wall) => building
        .GetWallsTouching(wall)
        .Select(currentWall => currentWall.GetIndex(building));

    static internal IEnumerable<int> GetWallIndicesTouching(this Building building, int index) => building.GetWallIndicesTouching(building.GetWall(index));
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

    static internal bool IsFloorValid(this Building building, int index) => building.GetFloor(index)?.IsValid() ?? false;

    static internal int AddFloor(this Building building, Curve2D polygon)
    {
        building.Floors.Add(new Floor(polygon));
        polygon.TryConnectChanged(building.ChangedCallable);
        building.EmitChanged();
        return building.Floors.Count - 1;
    }

    static internal int AddFloor(this Building building, Godot.Collections.Array data) => Floor.Deserialize(data).Match(
        ok: floor =>
        {
            int index = building.Floors.Count;
            building.Floors.Add(floor);
            building.EmitChanged();
            building.EmitSignal(nameof(building.FloorAdded), [index]);
            return index;
        },
        error: _ =>
        {
            GD.PrintS($"Unable to deserialize array data into Floor: {data}. Returning invalid index.");
            return -1;
        }
    );

    static internal void SetFloorPolygon(this Building building, int index, Curve2D polygon)
    {
        if (building.HasFloor(index))
        {
            building.Floors[index].Polygon = polygon;
            polygon.TryConnectChanged(building.ChangedCallable);
            building.EmitChanged();
        }
    }

    static internal void RemoveFloor(this Building building, int index)
    {
        if (!building.HasFloor(index)) return;
        building.Floors.RemoveAt(index);
        building.EmitChanged();
    }

    static internal MaterialMap GetFloorMaterials(this Building building, int index) => building.GetFloor(index)?.Materials ?? new MaterialMap();
    static internal StringName GetFloorMaterialID(this Building building, int index, StringName materialName)
    {
        if (building.HasFloor(index) && building.Floors[index].Materials.ContainsKey(materialName)) return building.Floors[index].Materials[materialName];
        return new StringName();
    }

    static internal StringName GetFloorID(this Building building, int index) => building.GetFloorMaterialID(index, "floor");

    static internal void SetFloorMaterials(this Building building, int index, MaterialMap value)
    {
        if (building.HasFloor(index)) building.Floors[index].Materials = value;
        building.EmitChanged();
    }

    static internal void SetFloorMaterialID(this Building building, int index, StringName materialName, StringName id)
    {
        if (building.HasFloor(index)) building.Floors[index].Materials.Add(materialName, id);
        building.EmitChanged();
    }

    static internal void SetFloorID(this Building building, int index, StringName id) => building.SetFloorMaterialID(index, "floor", id);

    static internal Curve2D? GetFloorPolygon(this Building building, int index) => building.GetFloor(index)?.Polygon;

    static internal Vector2[] TessellateFloor(this Building building, int index) => building.GetFloor(index)?.Tessellate() ?? [];

    static internal bool AreFloorsTouching(this Building building, int a, int b, float threshold) => building.GetFloor(a)?.IsTouching(building.GetFloor(b), threshold) ?? false;

    static internal int[] GetFloorsTouching(this Building building, int floorIndex, float threshold)
    {
        Floor? currentFloor = building.GetFloor(floorIndex);
        if (currentFloor == null) return [];
        return building.Floors.Select((floor, index) => currentFloor.IsTouching(floor, threshold) ? index : -1).Where(index => index != -1).ToArray();
    }

    static internal Rect2 GetFloorBoundingBox(this Building building, int index) => building.GetFloor(index)?.GetBoundingBox() ?? new();
    static internal Vector2 GetFloorCentroid(this Building building, int index) => building.GetFloor(index)?.GetCentroid() ?? new();

    static internal Vector2[] GetFloorPointPositions(this Building building, int index) => building.GetFloor(index)?.GetPointPositions() ?? [];

    static internal Vector2 SnapToFloor(this Building building, int index, Vector2 position, float threshold = -1) => position.Snap(
        building.GetFloor(index)?.Snap(position) ?? position,
        threshold
    );

    static internal Vector2 SnapToFloorSurface(this Building building, int index, Vector2 position, float threshold = -1) => position.Snap(
        building.GetFloor(index)?.SnapToSurface(position) ?? position,
        threshold
    );

    /// <summary>
    /// Returns a new position, snapped to the nearest floor point, if the distance is below threshold.
    /// </summary>
    static internal Vector2 SnapToFloors(this Building building, Vector2 position, float threshold = -1)
    {
        var closestPoint = Vector2.Inf;
        foreach (var floor in building.Floors)
            closestPoint = position.Closest(closestPoint, floor.Snap(position));
        return position.Snap(closestPoint, threshold);
    }

    static internal Vector2 SnapToFloorsSurface(this Building building, Vector2 position, float threshold = -1)
    {
        var closestPoint = Vector2.Inf;
        foreach (var floor in building.Floors)
            closestPoint = position.Closest(closestPoint, floor.SnapToSurface(position));
        return position.Snap(closestPoint, threshold);
    }
}

public static class BuildingExtensions
{
    static internal Vector2 GetCentroid(this Building building)
    {
        Vector2[] avgs = [
            building.Walls.Select(wall => wall.GetMidpoint()).Average(),
            building.Floors.Select(floor => floor.GetCentroid()).Average()
        ];
        return avgs.Average();
    }

    static internal Vector2 Snap(this Building building, Vector2 position, float threshold = -1) => position.Snap(
        position.Closest(
            building.SnapToFloors(position),
            building.SnapToWalls(position)
        ),
        threshold
    );

    static internal Vector2 SnapToSurface(this Building building, Vector2 position, float threshold = -1) => position.Snap(
        position.Closest(
            building.SnapToWallsSurface(position),
            building.SnapToFloorsSurface(position)
        ),
        threshold
    );

    static internal CompoundMesh GenerateMesh(this Building building)
    {
        var mesh = new CompoundMesh()
        {
            meshes = [
                ..building.Walls.Where(wall => wall.IsValid()).SelectMany(wall => wall.GenerateMeshes(building)),
                ..building.Floors.Where(floor => floor.IsValid()).SelectMany(floor => floor.GenerateMeshes(building))
            ]
        };
        // Connect("changed", new Callable(mesh, "generate"));
        mesh.generate();
        return mesh;
    }
}