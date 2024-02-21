using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PolygonEditor3D : PolygonEditor3DBase
{
    [Export]
    public Array<Vector2> Polygon = [];

    protected override void _AddPoint(int index, Vector2 position) => Polygon.Insert(index, position);
    protected override int _GetPointCount() => Polygon.Count;
    protected override Vector2 _GetPointPosition(int index) => Polygon[index];
    protected override void _RemovePoint(int index) => Polygon.RemoveAt(index);
    protected override void _SetPointPosition(int index, Vector2 position) => Polygon[index] = position;
}