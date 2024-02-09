using Godot;

[Tool]
[GlobalClass]
public partial class ExtrudeCurveMesh : ExtrudePointsMesh
{
    private int TessellationStages = 3;
    private Curve2D Curve;
    private Callable GenerateCallable;

    [Export]
    private Curve2D curve
    {
        get => Curve;
        set
        {
            Curve?.TryDisconnectChanged(GenerateCallable);
            value?.TryConnectChanged(GenerateCallable);
            Curve = value;
            generate();
        }
    }

    [Export]
    private int tessellation_stages
    {
        get => TessellationStages;
        set
        {
            TessellationStages = value;
            generate();
        }
    }

    public ExtrudeCurveMesh()
    {
        GenerateCallable = new Callable(this, "generate");
    }

    protected override void Clear()
    {
        Points = [];
        base.Clear();
    }

    protected bool BakePoints()
    {
        if (!CanBake()) return false;
        Points = Curve.Tessellate(TessellationStages);
        return Points.Length >= 2;
    }

    protected override bool Bake() => BakePoints() && base.Bake();

    protected override bool CanBake() => (
        Curve != null
        && Curve.PointCount >= 2
        && TessellationStages >= 0
        && Surface != null
    );
}