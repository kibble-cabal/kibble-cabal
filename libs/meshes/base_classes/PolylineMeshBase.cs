using Godot;
using System.Linq;

/// <summary>
/// Base class for generating polyline meshes based on sets of points.
/// </summary>
[Tool]
public abstract partial class PolylineMeshBase : PolygonMeshBase
{
    /* Private variables */
    protected float Thickness = 0.5f;
    protected bool Reverse = false;
    protected Vector2? JoinStart = null;
    protected Vector2? JoinEnd = null;

    /* Public variables */

    public Vector2? join_start
    {
        get => JoinStart;
        set
        {
            JoinStart = value;
            InternalMesh.Generate(this);
        }
    }

    public Vector2? join_end
    {
        get => JoinEnd;
        set
        {
            JoinEnd = value;
            InternalMesh.Generate(this);
        }
    }

    [Export]
    public bool reverse
    {
        get => Reverse;
        set
        {
            Reverse = value;
            InternalMesh.Generate(this);
        }
    }

    [Export]
    public float extrude_thickness
    {
        get => Thickness;
        set
        {
            Thickness = value;
            InternalMesh.Generate(this);
        }
    }

    protected Line GetLine() => new Line
    {
        Points = Reverse ? Points.Reverse().ToArray() : Points,
        ProjectionAxis = ProjectionAxis,
        ExtrudeDirection = ExtrudeDirection,
        ExtrudeAmount = Thickness,
        Flat = Flat,
        Invert = Invert,
        Surface = 0,
        CustomTransform = Transform3D.Identity,
        JoinStart = JoinStart,
        JoinEnd = JoinEnd
    };

    protected VolumePolyline GetPolyline() => new VolumePolyline
    {
        Points = Reverse ? Points.Reverse().ToArray() : Points,
        Thickness = Thickness,
        ExtrudeDirection = Vector3.Up,
        ExtrudeAmount = ExtrudeAmount,
        RenderTop = RenderTop,
        RenderBottom = RenderBottom,
        RenderEnds = RenderSides,
        JoinStart = JoinStart,
        JoinEnd = JoinEnd,
        Invert = Invert,
    };

    protected override IMeshComponent[] _GetComponents() => [Flat ? GetLine() : GetPolyline()];
}
