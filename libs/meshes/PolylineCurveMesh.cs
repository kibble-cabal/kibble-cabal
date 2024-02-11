using System.Linq;
using Godot;

#nullable enable

[Tool]
[GlobalClass]
public partial class PolylineCurveMesh : PolylineMeshBase
{
    /* Private variables */
    protected Curve2D? Curve;
    protected int TessellationStages = 3;
    protected float TessellationToleranceDegrees = 4;

    /* Public variables */

    [Export]
    private Curve2D? curve
    {
        get => Curve;
        set
        {
            Curve?.TryDisconnectChanged(new Callable(this, "_Generate"));
            value?.TryConnectChanged(new Callable(this, "_Generate"));
            Curve = value;
            InternalMesh.Generate(this);
        }
    }

    [Export]
    private int tessellation_stages
    {
        get => TessellationStages;
        set
        {
            TessellationStages = value;
            InternalMesh.Generate(this);
        }
    }

    [Export]
    private float tessellation_tolerance_degrees
    {
        get => TessellationToleranceDegrees;
        set
        {
            TessellationToleranceDegrees = value;
            InternalMesh.Generate(this);
        }
    }

    public PolylineCurveMesh()
    {
        this.InternalMesh = new(GetComponents, this);
    }

    /* Private methods */

    protected void _Generate() => InternalMesh.Generate(this);

    protected override Vector2[] _BakePoints() => Curve?.Tessellate(TessellationStages, TessellationToleranceDegrees) ?? [];

    protected override bool _CanBakePoints() => (
        Curve != null
        && Curve.PointCount >= 2
        && Curve.GetPointPositions().All(point => point.IsFinite())
    );
}
