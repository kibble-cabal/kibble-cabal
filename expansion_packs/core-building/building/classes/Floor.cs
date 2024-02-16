using System.Linq;
using Godot;

using Godot.Collections;


public record Floor : IGodotSerializable<Floor>, IBuildingComponent<Floor>
{
    public const float DefaultThickness = 0.2f;

    public MaterialMap Materials { get; set; }

    public Curve2D Polygon;
    public float Thickness = DefaultThickness;

    public StringName FloorID
    {
        get => Materials.ContainsKey("floor") ? Materials["floor"] : new();
        set => Materials.Add("floor", value);
    }

    public Floor() => this.Polygon = new();
    public Floor(Curve2D polygon) => this.Polygon = polygon ?? new();
    public Floor(Vector2[] points)
    {
        this.Polygon = new();
        points.ForEach(point => this.Polygon.AddPoint(point));
    }

    public int GetIndex(Building building) => building.Floors.IndexOf(this);
    public Vector2[] GetPointPositions() => this.Polygon?.GetPointPositions() ?? [];
    public void AddPoint(Vector2 point, Vector2? inHandle = null, Vector2? outHandle = null) => Polygon.AddPoint(point, inHandle, outHandle);
    public void InsertPoint(int index, Vector2 point, Vector2? inHandle = null, Vector2? outHandle = null) => Polygon.AddPoint(point, inHandle, outHandle, index);
    public void RemovePoint(int index) => Polygon.RemovePoint(index);

    public Vector2[] Tessellate()
    {
        if (Polygon == null || Polygon.PointCount < 3) return [];
        return Polygon.Tessellate(Building.TessellationStages, Building.TessellationToleranceDegrees);
    }

    public bool IsValid() => Polygon != null && Polygon.PointCount < 3 && !Geometry2D.TriangulatePolygon(Tessellate()).IsEmpty();

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

    public void MoveBy(Vector2 delta)
    {
        for (int i = 0; i < Polygon.PointCount; i++)
            Polygon.SetPointPosition(i, Polygon.GetPointPosition(i) + delta);
    }

    public Rect2 GetBoundingBox() => Polygon?.GetBakedPoints().GetBoundingBox() ?? new();
    public Vector2 ClosestPoint(Vector2 position) => this.Polygon?.ClosestPoint(position) ?? position;
    public Vector2 ClosestPointOnSurface(Vector2 position) => this.Polygon?.ClosestPointOnSurface(position) ?? position;

    public Mesh[] GenerateMeshes(Building building)
    {
        // TODO: Material
        var mat = new StandardMaterial3D { AlbedoColor = new Color(1, 0, 1) };
        return [new PolygonCurveMesh()
        {
            curve = Polygon,
            tessellation_stages = Building.TessellationStages,
            tessellation_tolerance_degrees = Building.TessellationToleranceDegrees,
            materials = [mat, mat, mat],
            extrude_height = Thickness,
            render_sides = true,
            render_top = true,
            render_bottom = false
        }];
    }

    public Array Serialize() => [Polygon, Materials];
    public static Result<Floor, GodotSerializationError> Deserialize(Array data) => Result.FromException(
        () => new Floor
        {
            Polygon = data[0].As<Curve2D>(),
            Materials = data[1]
        },
        onError: _ => GodotSerializationError.IncorrectData
    );
}