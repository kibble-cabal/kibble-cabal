using Godot;

using Godot.Collections;

public record Roof : IGodotSerializable<Roof>, IBuildingComponent<Roof>
{
    public MaterialMap Materials { get; set; }

    public Curve2D Polygon;
    public float Height = 1f;

    public StringName RoofID
    {
        get => Materials.ContainsKey("roof") ? Materials["roof"] : new();
        set => Materials.Add("roof", value);
    }

    public Roof() => this.Polygon = new();
    public Roof(Curve2D polygon) => this.Polygon = polygon ?? new();

    public int GetIndex(RBuilding building) => building.Roofs.IndexOf(this);
    public Vector2[] GetPointPositions() => this.Polygon?.GetPointPositions() ?? [];
    public void AddPoint(Vector2 point, Vector2? inHandle = null, Vector2? outHandle = null) => Polygon.AddPoint(point, inHandle, outHandle);
    public void InsertPoint(int index, Vector2 point, Vector2? inHandle = null, Vector2? outHandle = null) => Polygon.AddPoint(point, inHandle, outHandle, index);
    public void RemovePoint(int index) => Polygon.RemovePoint(index);

    public Vector2[] Tessellate()
    {
        if (Polygon == null || Polygon.PointCount < 3) return [];
        return Polygon.Tessellate(RBuilding.TessellationStages, RBuilding.TessellationToleranceDegrees);
    }

    public bool IsValid() => Polygon != null && Polygon.PointCount < 3 && !Geometry2D.TriangulatePolygon(Tessellate()).IsEmpty();

    public bool IsTouching(Roof other, float threshold)
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

    public Mesh[] GenerateMeshes(RBuilding building)
    {
        throw new System.NotImplementedException();
    }

    public Array Serialize() => [Polygon, Materials];
    public static Result<Roof, GodotSerializationError> Deserialize(Array data) => Result.FromException(
        () => new Roof
        {
            Polygon = data[0].As<Curve2D>(),
            Materials = data[1],
        },
        onError: _ => GodotSerializationError.IncorrectData
    );
}