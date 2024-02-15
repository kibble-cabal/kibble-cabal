using Godot;

#nullable enable

/// <summary>
/// Base class for generating polygon meshes based on sets of points.
/// </summary>
[Tool]
public abstract partial class PolygonMeshBase : PackedVector2ArrayMesh
{
    /* Private variables */
    protected Vector3.Axis ProjectionAxis = Vector3.Axis.Y;
    protected float ExtrudeAmount = 1.0f;
    protected Vector3 ExtrudeDirection = Vector3.Up;
    protected bool Flat = false;
    protected bool RenderTop = true;
    protected bool RenderBottom = true;
    protected bool RenderSides = true;

    /* Public variables */

    [Export]
    public Vector3.Axis projection_axis
    {
        get => ProjectionAxis;
        set
        {
            ProjectionAxis = value;
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
    public bool render_sides
    {
        get => RenderSides;
        set
        {
            RenderSides = value;
            InternalMesh.Generate(this);
        }
    }

    /* Private methods */

    protected Polygon GetPolygon() => new Polygon
    {
        Points = Points,
        Invert = Invert,
        ProjectionAxis = ProjectionAxis,
        CustomTransform = Transform3D.Identity
    };

    protected VolumePolygon GetVolumePolygon() => new VolumePolygon
    {
        Points = Points,
        Invert = Invert,
        ExtrudeAmount = ExtrudeAmount,
        ExtrudeDirection = ExtrudeDirection,
        RenderTop = RenderTop,
        RenderBottom = RenderBottom,
        RenderSides = RenderSides
    };

    protected override Vector2[] _BakePoints() => Points;
    protected override IMeshComponent[] _GetComponents() => [Flat ? GetPolygon() : GetVolumePolygon()];
}
