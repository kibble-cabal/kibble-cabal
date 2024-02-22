using Godot;
using Godot.Collections;

[GlobalClass]
public partial class PolygonEditor3D : PolygonEditor3DBase
{
    [Export]
    public Array<Vector2> Polygon = [];

    [Export]
    public Array<Vector2> InHandlePositions = [];

    [Export]
    public Array<Vector2> OutHandlePositions = [];

    protected override void _AddPoint(int index, Vector2 position)
    {
        Polygon.Insert(index, position);
        InHandlePositions.Insert(index, Vector2.Zero);
        OutHandlePositions.Insert(index, Vector2.Zero);
    }
    protected override Vector2 _GetInHandlePosition(int index) => InHandlePositions[index];
    protected override Vector2 _GetOutHandlePosition(int index) => OutHandlePositions[index];
    protected override int _GetPointCount() => Polygon.Count;
    protected override Vector2 _GetPointPosition(int index) => Polygon[index];
    protected override void _RemovePoint(int index)
    {
        Polygon.RemoveAt(index);
        InHandlePositions.RemoveAt(index);
        OutHandlePositions.RemoveAt(index);
    }
    protected override void _SetInHandlePosition(int index, Vector2 position) => InHandlePositions[index] = position;
    protected override void _SetOutHandlePosition(int index, Vector2 position) => OutHandlePositions[index] = position;
    protected override void _SetPointPosition(int index, Vector2 position) => Polygon[index] = position;
}