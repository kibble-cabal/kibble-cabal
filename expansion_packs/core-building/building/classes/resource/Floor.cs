using Godot;

using Godot.Collections;
using MaterialMap = Godot.Collections.Dictionary<Godot.StringName, Godot.StringName>;


public record Floor
{
    public Curve2D Polygon;
    public MaterialMap Materials = [];

    public StringName FloorID
    {
        get => Materials.ContainsKey("floor") ? Materials["floor"] : new();
        set => Materials["floor"] = value;
    }

    public Floor(Curve2D polygon) => this.Polygon = polygon;

    public void AddPoint(Vector2 point) => Polygon.AddPoint(point);
    public void AddPoint(Vector2 point, Vector2 inHandle, Vector2 outHandle) => Polygon.AddPoint(point, inHandle, outHandle);

    public void InsertPoint(int index, Vector2 point) => Polygon.AddPoint(point, null, null, index);
    public void InsertPoint(int index, Vector2 point, Vector2 inHandle, Vector2 outHandle) => Polygon.AddPoint(point, inHandle, outHandle, index);

    public void RemovePoint(int index) => Polygon.RemovePoint(index);

    public Vector2[] Tessellate(int maxStages, float toleranceDegrees) => Polygon.Tessellate(maxStages, toleranceDegrees);

    public bool IsValid() => Polygon.PointCount > 2;

    public bool IsTouching(Floor other)
    {
        throw new System.NotImplementedException();
    }

    public Array ToData()
    {
        return [Polygon, Materials];
    }

    public static Floor FromData(Array data)
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