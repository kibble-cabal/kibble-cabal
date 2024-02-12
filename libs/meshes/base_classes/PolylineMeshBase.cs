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
    protected Vector3 ExtrudeDirection = Vector3.Up;
    protected float ExtrudeAmount = 1.0f;
    protected bool RenderTop = true;
    protected bool RenderBottom = true;
    protected bool RenderEnds = true;
    protected bool Reverse = false;
    protected bool Flat = false;
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
    public bool flat
    {
        get => Flat;
        set
        {
            Flat = value;
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

    [ExportGroup("Extrusion", "extrude_")]

    [Export]
    public float extrude_height
    {
        get => ExtrudeAmount;
        set
        {
            ExtrudeAmount = value;
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

    [Export]
    public Vector3 extrude_direction
    {
        get => ExtrudeDirection;
        set
        {
            ExtrudeDirection = value;
            InternalMesh.Generate(this);
        }
    }


    [ExportGroup("Rendering", "render_")]

    [Export]
    public bool render_top
    {
        get => RenderTop;
        set
        {
            RenderTop = value;
            InternalMesh.Generate(this);
        }
    }

    [Export]
    public bool render_bottom
    {
        get => RenderBottom;
        set
        {
            RenderBottom = value;
            InternalMesh.Generate(this);
        }
    }

    [Export]
    public bool render_ends
    {
        get => RenderEnds;
        set
        {
            RenderEnds = value;
            InternalMesh.Generate(this);
        }
    }

    protected Line GetLine() => new Line
    {
        Points = Points,
        ProjectionAxis = ProjectionAxis,
        ExtrudeDirection = ExtrudeDirection,
        ExtrudeAmount = ExtrudeAmount,
        Flat = Flat,
        Invert = Invert,
        Surface = 0,
        CustomTransform = Transform3D.Identity
    };

    protected VolumePolyline GetPolyline() => new VolumePolyline
    {
        Points = Reverse ? Points.Reverse().ToArray() : Points,
        Thickness = Thickness,
        ExtrudeDirection = Vector3.Up,
        ExtrudeAmount = ExtrudeAmount,
        RenderTop = RenderTop,
        RenderBottom = RenderBottom,
        RenderEnds = RenderEnds,
        JoinStart = JoinStart,
        JoinEnd = JoinEnd
    };

    protected override IMeshComponent[] _GetComponents() => [Thickness < F.AlmostZero ? GetLine() : GetPolyline()];
}
