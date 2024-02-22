using Godot;

[GlobalClass]
public partial class CurveEditor3D : PolygonEditor3DBase
{
    private Curve2D? _curve;

    [Export]
    public Curve2D? Curve
    {
        get => _curve;
        set
        {
            _curve = value;
            UpdatePoints();
        }
    }

    protected override void _AddPoint(int index, Vector2 position) => Curve?.AddPoint(position, null, null, index);
    protected override Vector2 _GetInHandlePosition(int index) => Curve?.GetPointIn(index) ?? Vector2.Zero;
    protected override Vector2 _GetOutHandlePosition(int index) => Curve?.GetPointOut(index) ?? Vector2.Zero;
    protected override int _GetPointCount() => Curve?.PointCount ?? 0;
    protected override Vector2 _GetPointPosition(int index) => Curve?.GetPointPosition(index) ?? default;
    protected override void _RemovePoint(int index) => Curve?.RemovePoint(index);
    protected override void _SetInHandlePosition(int index, Vector2 position) => Curve?.SetPointIn(index, position);
    protected override void _SetOutHandlePosition(int index, Vector2 position) => Curve?.SetPointOut(index, position);
    protected override void _SetPointPosition(int index, Vector2 position) => Curve?.SetPointPosition(index, position);
}