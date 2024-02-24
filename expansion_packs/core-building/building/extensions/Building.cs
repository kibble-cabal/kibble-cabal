using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

/// <summary>
/// This class contains functions related to building walls that should not be exposed directly to GDScript.
/// </summary>
internal static class BuildingWallExtensions
{
    static internal WallRef? GetWallRef(this RBuilding building, int index) => building.Has<Wall>(index) ? new(building, index) : null;
    static internal int Add<T>(this RBuilding building, Vector2 start, Vector2 end, Vector2? startHandle = null, Vector2? endHandle = null) where T : Wall => building.Add<Wall>(new Wall(start, end, startHandle, endHandle));
    static internal int[] Add<T>(this RBuilding building, Vector2[] points) where T : Wall
    {
        Wall[] walls = new Wall[points.Length - 1];
        for (int i = 0; i < points.Length - 1; i++)
            walls[i] = new Wall(points[i], points[i + 1]);
        return building.Add<Wall>(walls);
    }
    static internal int[] Add<T>(this RBuilding building, Curve2D curve) where T : Wall
    {
        Wall[] walls = new Wall[curve.PointCount - 1];
        for (int i = 0; i < curve.PointCount - 1; i++)
            walls[i] = new Wall(curve.GetPointPosition(i), curve.GetPointOut(i), curve.GetPointPosition(i + 1), curve.GetPointIn(i + 1));
        return building.Add<Wall>(walls);
    }
    static internal void Insert<T>(this RBuilding building, int index, Vector2 start, Vector2 end, Vector2? startHandle = null, Vector2? endHandle = null) where T : Wall => building.Insert<Wall>(index, new Wall(start, end, startHandle, endHandle));
    static internal void SetWallPositions(this RBuilding building, int index, Vector2 start, Vector2 end)
    {
        building.SetWallStart(index, start);
        building.SetWallEnd(index, end);
    }
    static internal void SetWallHandles(this RBuilding building, int index, Vector2 startHandle, Vector2 endHandle)
    {
        building.SetWallStartHandle(index, startHandle);
        building.SetWallEndHandle(index, endHandle);
    }
    static internal void SetWall(this RBuilding building, int index, Vector2 start, Vector2 startHandle, Vector2 end, Vector2 endHandle)
    {
        building.SetWallPositions(index, start, end);
        building.SetWallHandles(index, startHandle, endHandle);
    }
    static internal Vector2 GetWallStart(this RBuilding building, int index) => building.Get<Wall>(index)?.Start ?? Vector2.Inf;
    static internal void SetWallStart(this RBuilding building, int index, Vector2 position)
    {
        if (building.Get<Wall>(index) is Wall wall) wall.Start = position;
        building.EmitChanged();
    }
    static internal Vector2 GetWallEnd(this RBuilding building, int index) => building.Get<Wall>(index)?.End ?? Vector2.Inf;
    static internal void SetWallEnd(this RBuilding building, int index, Vector2 position)
    {
        if (building.Get<Wall>(index) is Wall wall) wall.End = position;
        building.EmitChanged();
    }
    static internal Vector2 GetWallStartHandle(this RBuilding building, int index) => building.Get<Wall>(index)?.StartHandle ?? Vector2.Inf;
    static internal void SetWallStartHandle(this RBuilding building, int index, Vector2 position)
    {
        if (building.Get<Wall>(index) is Wall wall) wall.StartHandle = position;
        building.EmitChanged();
    }

    static internal Vector2 GetWallEndHandle(this RBuilding building, int index) => building.Get<Wall>(index)?.EndHandle ?? Vector2.Inf;
    static internal void SetWallEndHandle(this RBuilding building, int index, Vector2 position)
    {
        if (building.Get<Wall>(index) is Wall wall) wall.EndHandle = position;
        building.EmitChanged();
    }
    static internal float GetWallHeight(this RBuilding building, int index) => building.Get<Wall>(index)?.Height ?? Wall.DefaultHeight;
    static internal void SetWallHeight(this RBuilding building, int index, float value)
    {
        if (building.Get<Wall>(index) is Wall wall) wall.Height = value;
        building.EmitChanged();
    }
    static internal float GetWallThickness(this RBuilding building, int index) => building.Get<Wall>(index)?.Thickness ?? Wall.DefaultThickness;
    static internal void SetWallThickness(this RBuilding building, int index, float value)
    {
        if (building.Get<Wall>(index) is Wall wall) wall.Thickness = value;
        building.EmitChanged();
    }
    /// <summary>
    /// Modifies all connected walls' heights.
    /// </summary>
    static internal void FillWallHeight(this RBuilding building, int index, float value)
    {
        building.SelectConnected<Wall>(index, (wall, _) => wall.Height = value);
        building.EmitChanged();
    }
    /// <summary>
    /// Modifies all connected walls' thickness.
    /// </summary>
    static internal void FillWallThickness(this RBuilding building, int index, float value)
    {
        building.SelectConnected<Wall>(index, (wall, _) => wall.Thickness = value);
        building.EmitChanged();
    }
    static internal StringName? GetWallInteriorID(this RBuilding building, int index) => building.GetMaterialID<Wall>(index, "interior");
    static internal void SetWallInteriorID(this RBuilding building, int index, StringName id) => building.SetMaterialID<Wall>(index, "interior", id);
    static internal StringName? GetWallExteriorID(this RBuilding building, int index) => building.GetMaterialID<Wall>(index, "exterior");
    static internal void SetWallExteriorID(this RBuilding building, int index, StringName id) => building.SetMaterialID<Wall>(index, "exterior", id);
    static internal Vector2 GetWallMidpoint(this RBuilding building, int index) => building.Get<Wall>(index)?.GetMidpoint() ?? Vector2.Inf;
}

/// <summary>
/// This class contains functions related to building floors that should not be exposed directly to GDScript.
/// </summary>
internal static class BuildingFloorExtensions
{
    static internal FloorRef? GetFloorRef(this RBuilding building, int index) => building.Has<Floor>(index) ? new(building, index) : null;
    static internal int Add<T>(this RBuilding building, Curve2D polygon) where T : Floor => building.Add<Floor>(new Floor(polygon));
    static internal int Add<T>(this RBuilding building, Vector2[] points) where T : Floor => building.Add<Floor>(new Floor(points));
    static internal void SetFloorPolygon(this RBuilding building, int index, Curve2D polygon)
    {
        if (building.Has<Floor>(index))
        {
            building.Floors[index].Polygon = polygon;
            polygon.TryConnectChanged(building.ChangedCallable);
            building.EmitChanged();
        }
    }
    static internal float GetFloorThickness(this RBuilding building, int index) => building.Get<Floor>(index)?.Thickness ?? Floor.DefaultThickness;
    static internal void SetFloorThickness(this RBuilding building, int index, float value)
    {
        if (building.Get<Floor>(index) is Floor floor)
            floor.Thickness = value;
        building.EmitChanged();
    }
    static internal StringName GetFloorID(this RBuilding building, int index) => building.GetMaterialID<Floor>(index, "floor") ?? new();
    static internal void SetFloorID(this RBuilding building, int index, StringName id) => building.SetMaterialID<Floor>(index, "floor", id);
    static internal Curve2D? GetFloorPolygon(this RBuilding building, int index) => building.Get<Floor>(index)?.Polygon;
    static internal Vector2[] GetFloorPointPositions(this RBuilding building, int index) => building.Get<Floor>(index)?.GetPointPositions() ?? [];
}

public static class BuildingExtensions
{
    static internal bool Has<T>(this RBuilding building, int index) where T : IBuildingComponent<T> => building.GetList<T>().Count > index && index >= 0;
    static internal T? Get<T>(this RBuilding building, int index) where T : IBuildingComponent<T> => building.GetList<T>().ElementAtOrDefault(index);
    static internal int[] Add<T>(this RBuilding building, T[] components) where T : IBuildingComponent<T> => components.Select(building.Add<T>).ToArray();
    static internal int Add<T>(this RBuilding building, T component) where T : IBuildingComponent<T>
    {
        int index = building.Count<T>();
        var list = building.GetList<T>();
        list.Add(component);
        building.EmitSignal(RBuilding.AddSignalName<T>());
        building.EmitChanged();
        return index;
    }
    static internal int Add<T>(this RBuilding building, Godot.Collections.Array data) where T : IBuildingComponent<T>, IGodotSerializable<T> => T.Deserialize(data).Match(
        ok: building.Add<T>,
        error: _ =>
        {
            GD.PushError($"Unable to deserialize array data into {typeof(T).Name}: {data}. Returning invalid index.");
            return -1;
        }
    );

    static internal void Insert<T>(this RBuilding building, int index, T[] components) where T : IBuildingComponent<T> => components.Reverse().ForEach(component => building.Insert<T>(index, component));
    static internal void Insert<T>(this RBuilding building, int index, T component) where T : IBuildingComponent<T>
    {
        var list = building.GetList<T>();
        if (index > list.Count || index < 0) return;
        list.Insert(index, component);
        building.EmitSignal(RBuilding.AddSignalName<T>());
        building.EmitChanged();
    }
    static internal void Insert<T>(this RBuilding building, int index, Godot.Collections.Array data) where T : IBuildingComponent<T>, IGodotSerializable<T> => T.Deserialize(data).Match(
        ok: component => building.Insert<T>(index, component),
        error: _ => GD.PushError($"Unable to deserialize array data into {typeof(T).Name}: {data}.")
    );

    static internal bool IsValid<T>(this RBuilding building, int index) where T : IBuildingComponent<T> => building.Get<T>(index)?.IsValid() ?? false;
    static internal IEnumerable<T> GetValid<T>(this RBuilding building) where T : IBuildingComponent<T> => building.GetList<T>().Where(component => component.IsValid());
    static internal int GetIndex<T>(this RBuilding building, T component) where T : IBuildingComponent<T> => component.GetIndex(building);
    static internal Vector2[] Tessellate<T>(this RBuilding building, int index) where T : IBuildingComponent<T> => building.Get<T>(index)?.Tessellate() ?? [];
    static internal bool IsTouching<T>(this RBuilding building, int a, int b, float threshold = -1) where T : IBuildingComponent<T> => building.Has<T>(a) && building.Has<T>(b) && building.Get<T>(a)!.IsTouching(building.Get<T>(b)!, threshold);
    static internal Rect2? GetBoundingBox<T>(this RBuilding building, int index) where T : IBuildingComponent<T> => building.Get<T>(index)?.GetBoundingBox();
    static internal Vector2? ClosestPoint<T>(this RBuilding building, int index, Vector2 position) where T : IBuildingComponent<T> => building.Get<T>(index)?.ClosestPoint(position);
    static internal Vector2? ClosestPointOnSurface<T>(this RBuilding building, int index, Vector2 position) where T : IBuildingComponent<T> => building.Get<T>(index)?.ClosestPointOnSurface(position);
    static internal Mesh[] GenerateMeshes<T>(this RBuilding building, int index) where T : IBuildingComponent<T> => building.Get<T>(index)?.GenerateMeshes(building) ?? [];
    static internal void MoveBy<T>(this RBuilding building, int index, Vector2 delta) where T : IBuildingComponent<T>
    {
        building.Get<T>(index)?.MoveBy(delta);
        building.EmitChanged();
    }
    static internal void MoveBy<T>(this RBuilding building, IEnumerable<int> indices, Vector2 delta) where T : IBuildingComponent<T> => indices.ForEach(i => building.MoveBy<T>(i, delta));
    static internal void MoveBy(this RBuilding building, Vector2 delta)
    {
        building.MoveBy<Wall>(GD.Range(0, building.Walls.Count), delta);
        building.MoveBy<Floor>(GD.Range(0, building.Floors.Count), delta);
        building.MoveBy<Roof>(GD.Range(0, building.Roofs.Count), delta);
    }
    static internal MaterialMap? GetMaterials<T>(this RBuilding building, int index) where T : IBuildingComponent<T> => building.Get<T>(index)?.Materials;
    static internal void SetMaterials<T>(this RBuilding building, int index, MaterialMap value) where T : IBuildingComponent<T>
    {
        if (building.Get<T>(index) is T component)
            component.Materials = value;
        building.EmitChanged();
    }
    static internal StringName? GetMaterialID<T>(this RBuilding building, int index, StringName materialName) where T : IBuildingComponent<T> => building.Get<T>(index)?.Materials.Get(materialName);
    static internal void SetMaterialID<T>(this RBuilding building, int index, StringName materialName, StringName value) where T : IBuildingComponent<T>
    {
        if (building.Get<T>(index) is T component)
            component.Materials.Add(materialName, value);
        building.EmitChanged();
    }

    public static Vector2? GetCentroid<T>(this RBuilding building, int index) where T : IBuildingComponent<T> => building.GetBoundingBox<T>(index)?.GetCenter();

    public static Vector2 Snap<T>(this RBuilding building, int index, Vector2 position, float threshold = -1) where T : IBuildingComponent<T> => position.Snap(building.Get<T>(index)?.ClosestPoint(position) ?? Vector2.Inf, threshold);
    public static Vector2 Snap<T>(this RBuilding building, Vector2 position, float threshold = -1) where T : IBuildingComponent<T>
    {
        var closestPoint = Vector2.Inf;
        foreach (var component in building.GetList<T>())
            closestPoint = position.Closest(closestPoint, component.Snap(position));
        return position.Snap(closestPoint, threshold);
    }

    public static Vector2 SnapToSurface<T>(this RBuilding building, int index, Vector2 position, float threshold = -1) where T : IBuildingComponent<T> => position.Snap(building.Get<T>(index)?.ClosestPointOnSurface(position) ?? Vector2.Inf, threshold);
    public static Vector2 SnapToSurface<T>(this RBuilding building, Vector2 position, float threshold = -1) where T : IBuildingComponent<T>
    {
        var closestPoint = Vector2.Inf;
        foreach (var component in building.GetList<T>())
            closestPoint = position.Closest(closestPoint, component.SnapToSurface(position));
        return position.Snap(closestPoint, threshold);
    }

    /// <summary>
    /// Removes all invalid components from this building. See <see cref="IBuildingComponent.IsValid"/> 
    /// </summary>
    static internal void RemoveInvalid<T>(this RBuilding building) where T : IBuildingComponent<T> => building.GetList<T>().Where(component => !component.IsValid()).ForEach(building.Remove<T>);

    static internal void Remove<T>(this RBuilding building, int index) where T : IBuildingComponent<T>
    {
        var list = building.GetList<T>();
        if (!building.Has<T>(index)) return;
        var data = list[index].Serialize();
        list.RemoveAt(index);
        building.EmitChanged();
        building.EmitSignal(RBuilding.RemoveSignalName<T>(), [index, data]);
    }

    static internal void Remove<T>(this RBuilding building, T component) where T : IBuildingComponent<T> => building.Remove<T>(building.GetIndex<T>(component));

    static internal int[] GetAllConnected<T>(this RBuilding building, int index) where T : class, IBuildingComponent<T>
    {
        int[] connected = [], added = [index];
        while (added.Length > 0)
        {
            var adding = added
                .SelectMany(i => building.GetIndicesTouching<T>(i))
                .Except(connected)
                .Except(added)
                .ToArray();
            connected = [.. connected, .. added];
            added = adding;
        }
        return connected;
    }

    static internal void SelectConnected<T>(this RBuilding building, int index, Action<T, int> predicate) where T : class, IBuildingComponent<T> => building.GetAllConnected<T>(index).ForEach(i => predicate(building.Get<T>(i)!, i));
    static internal IEnumerable<R> SelectConnected<T, R>(this RBuilding building, int index, Func<T, int, R> predicate) where T : class, IBuildingComponent<T> => building.GetAllConnected<T>(index).Select(i => predicate(building.Get<T>(i)!, i));

    static internal void RemoveConnected<T>(this RBuilding building, int index) where T : class, IBuildingComponent<T>
    {
        building.GetAllConnected<T>(index).Distinct().OrderByDescending(i => i).ForEach(building.Remove<T>);
        building.EmitChanged();
    }

    static internal void MoveConnectedBy<T>(this RBuilding building, int index, Vector2 delta) where T : class, IBuildingComponent<T>
    {
        building.SelectConnected<T>(index, (component, _) => component.MoveBy(delta));
        building.EmitChanged();
    }

    static internal IEnumerable<T> GetTouching<T>(this RBuilding building, T? component, float threshold = F.AlmostZero) where T : class, IBuildingComponent<T> => component == null ? [] : building.GetList<T>()
        .Where(c => c != component && c.IsValid())
        .Select(c => c.IsTouching(component, threshold) ? c : null)
        .WhereNotNull();

    static internal IEnumerable<int> GetIndicesTouching<T>(this RBuilding building, T? component, float threshold = F.AlmostZero) where T : class, IBuildingComponent<T> => building
        .GetTouching<T>(component, threshold)
        .Select(c => c.GetIndex(building));

    static internal IEnumerable<int> GetIndicesTouching<T>(this RBuilding building, int index, float threshold = F.AlmostZero) where T : class, IBuildingComponent<T> => building.GetIndicesTouching<T>(building.Get<T>(index), threshold);

    static internal Vector2 GetCentroid(this RBuilding building)
    {
        Vector2[] avgs = [
            building.Walls.Select(wall => wall.GetMidpoint()).Average(),
            building.Floors.Select(floor => floor.GetCentroid()).Average()
        ];
        return avgs.Average();
    }

    static internal Vector2 Snap(this RBuilding building, Vector2 position, float threshold = -1) => position.Snap(
        position.Closest(building.Snap<Wall>(position), building.Snap<Floor>(position)),
        threshold
    );

    static internal Vector2 SnapToSurface(this RBuilding building, Vector2 position, float threshold = -1) => position.Snap(
        position.Closest(building.SnapToSurface<Wall>(position), building.SnapToSurface<Floor>(position)),
        threshold
    );

    static internal BuildingMesh GetMesh(this RBuilding building) => new BuildingMesh(building);

    static internal CompoundMesh GenerateMesh(this RBuilding building)
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