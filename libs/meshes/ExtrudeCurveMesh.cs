using Godot;

[Tool]
[GlobalClass]
public partial class ExtrudeCurveMesh : ExtrudePackedVector2ArrayMesh
{
    private int TessellationStages = 3;
    private float TessellationToleranceDegrees = 4;
    private Curve2D Curve;
    private Callable GenerateCallable;

    [Export]
    public Curve2D curve
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

    public ExtrudeCurveMesh()
    {
        this.InternalMesh = new(GetTriangles, this);
        GenerateCallable = new Callable(this, "_Generate");
    }

    internal void _Generate() => InternalMesh.Generate(this);

    internal override bool _BakePoints()
    {
        Points = Curve?.Tessellate(TessellationStages, TessellationToleranceDegrees) ?? [];
        return Points.Length >= 2;
    }
}