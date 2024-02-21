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
            _curve?.TryDisconnectChanged(Callable.From(UpdatePoints));
            _curve = value;
            _curve?.TryConnectChanged(Callable.From(UpdatePoints));
            UpdatePoints();
        }
    }

    protected override void _AddPoint(int index, Vector2 position) => Curve?.AddPoint(position, null, null, index);
    protected override int _GetPointCount() => Curve?.PointCount ?? 0;
    protected override Vector2 _GetPointPosition(int index) => Curve?.GetPointPosition(index) ?? default;
    protected override void _RemovePoint(int index) => Curve?.RemovePoint(index);
    protected override void _SetPointPosition(int index, Vector2 position) => Curve?.SetPointPosition(index, position);
}