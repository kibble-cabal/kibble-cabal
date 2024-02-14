using System.Linq;
using Godot;

using Godot.Collections;
using MaterialMap = Godot.Collections.Dictionary<Godot.StringName, Godot.StringName>;


public record Floor : IGodotSerializable<Floor>
{
    public Curve2D Polygon;
    public MaterialMap Materials = [];

    public StringName FloorID
    {
        get => Materials.ContainsKey("floor") ? Materials["floor"] : new();
        set => Materials["floor"] = value;
    }

    public Floor(Curve2D polygon) => this.Polygon = polygon ?? new();

    public Vector2[] GetPointPositions() => this.Polygon?.GetPointPositions() ?? [];

    public void AddPoint(Vector2 point) => Polygon.AddPoint(point);
    public void AddPoint(Vector2 point, Vector2 inHandle, Vector2 outHandle) => Polygon.AddPoint(point, inHandle, outHandle);

    public void InsertPoint(int index, Vector2 point) => Polygon.AddPoint(point, null, null, index);
    public void InsertPoint(int index, Vector2 point, Vector2 inHandle, Vector2 outHandle) => Polygon.AddPoint(point, inHandle, outHandle, index);

    public void RemovePoint(int index) => Polygon.RemovePoint(index);

    public Vector2[] Tessellate()
    {
        if (Polygon == null || Polygon.PointCount < 3) return [];
        Vector2[] points = Polygon.Tessellate(Building.TessellationStages, Building.TessellationToleranceDegrees);
        if (Polygon.PointCount >= 2 && points.Length > 0)
            points = [.. points, .. Tessellator.tessellate(
                points[^1],
                points[0],
                Polygon.GetPointOut(Polygon.PointCount - 1),
                Polygon.GetPointIn(0)
            )];
        return points;
    }

    public Vector2[] Triangulate()
    {
        var points = Tessellate();
        if (points.Length >= 2) return Geometry2D.TriangulatePolygon(points).Select(i => points[i]).ToArray();
        return [];
    }

    public bool IsValid()
    {
        if (Polygon == null || Polygon.PointCount < 3) return false;
        return !Geometry2D.TriangulatePolygon(Tessellate()).IsEmpty();
    }

    public bool IsTouching(Floor other, float threshold)
    {
        var polygonA = Tessellate();
        var polygonB = other.Tessellate();
        foreach (var point in polygonA)
            if (Geometry2D.IsPointInPolygon(point, polygonB)) return true;

        foreach (var aPoint in polygonA)
            foreach (var bPoint in polygonB)
            {
                var dist = Mathf.Abs(aPoint.DistanceTo(bPoint));
                if (dist < threshold) return true;
            }

        return false;
    }

    public Vector2 ClosestPoint(Vector2 position) => this.Polygon?.ClosestPoint(position) ?? position;
    public Vector2 ClosestPointOnSurface(Vector2 position) => this.Polygon?.ClosestPointOnSurface(position) ?? position;
    public Vector2 Snap(Vector2 position, float threshold) => position.Snap(ClosestPoint(position), threshold);
    public Vector2 Snap(Vector2 position) => Snap(position, -1);
    public Vector2 SnapToSurface(Vector2 position, float threshold) => position.Snap(ClosestPointOnSurface(position), threshold);
    public Vector2 SnapToSurface(Vector2 position) => SnapToSurface(position, -1);

    public PolygonCurveMesh GenerateMesh()
    {
        // TODO: Material
        return new PolygonCurveMesh()
        {
            curve = Polygon,
            tessellation_stages = Building.TessellationStages,
            tessellation_tolerance_degrees = Building.TessellationToleranceDegrees,
            materials = [new StandardMaterial3D { AlbedoColor = new Color(1, 0, 1) }]
        };
    }

    public Array Serialize() => [Polygon, Materials];
    public static Floor Deserialize(Array data)
    {
        Floor floor = new Floor(new Curve2D());
        if (data.Count >= 2)
        {
            floor.Polygon = data[0].As<Curve2D>();
            floor.Materials = data[1].As<MaterialMap>();
        }
        return floor;
    }
}