using Godot;

[Tool]
[GlobalClass]
public partial class PolylineCurveMesh : PolylineMeshBase
{
    private int TessellationStages = 3;
    private float TessellationToleranceDegrees = 4;
    private Curve2D? Curve;

    [Export]
    public Curve2D? curve
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
    public int tessellation_stages
    {
        get => TessellationStages;
        set
        {
            TessellationStages = value;
            InternalMesh.Generate(this);
        }
    }

    [Export]
    public float tessellation_tolerance_degrees
    {
        get => TessellationToleranceDegrees;
        set
        {
            TessellationToleranceDegrees = value;
            InternalMesh.Generate(this);
        }
    }

    protected void _Generate() => InternalMesh.Generate(this);

    protected override Vector2[] _BakePoints() => Curve?.Tessellate(TessellationStages, TessellationToleranceDegrees) ?? [];
}