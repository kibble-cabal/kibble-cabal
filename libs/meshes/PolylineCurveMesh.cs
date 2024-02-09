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
    protected Callable GenerateCallable;

    /* Public variables */

    [Export]
    private Curve2D? curve
    {
        get => Curve;
        set
        {
            Curve?.TryDisconnectChanged(GenerateCallable);
            value?.TryConnectChanged(GenerateCallable);
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
        InternalMesh = new(GetTriangles, this);
        GenerateCallable = new Callable(this, "_Generate");
    }

    /* Private methods */

    internal void _Generate() => InternalMesh.Generate(this);

    internal override bool _BakePoints()
    {
        if (!CanBake()) return false;
        Points = Curve?.Tessellate(TessellationStages, TessellationToleranceDegrees) ?? [];
        return base._BakePoints();
    }

    internal bool CanBake() => (
        Curve != null
        && Curve.PointCount >= 2
        && Curve.GetPointPositions().All(point => point.IsFinite())
    );
}
