using Godot;

/// <summary>
/// Base class for generating polyline meshes based on sets of points.
/// </summary>
[Tool]
public abstract partial class PolylineMeshBase : PolygonMeshBase
{
    protected float Thickness = 0.1f;
    protected Vector3.Axis ZeroAxis = Vector3.Axis.Y;

    [Export]
    public float thickness
    {
        get => Thickness;
        set
        {
            Thickness = value;
            InternalMesh.Generate(this);
        }
    }

    [Export]
    public Vector3.Axis zero_axis
    {
        get => ZeroAxis;
        set
        {
            ZeroAxis = value;
            InternalMesh.Generate(this);
        }
    }

    protected Polyline GetPolyline() => new Polyline { Points = Points, Thickness = Thickness, ZeroAxis = ZeroAxis, Offset = Vector3.Zero, Invert = Invert };

    protected override IMeshComponent[] _GetComponents() => [GetPolyline()];
    // protected override Triangle[] _GetTriangles() => GetPolyline().GetTriangles();
}
