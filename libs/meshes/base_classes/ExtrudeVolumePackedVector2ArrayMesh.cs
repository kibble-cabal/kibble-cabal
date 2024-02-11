using Godot;

#nullable enable

/// <summary>
/// Base class for generating extruded meshes based on sets of points.
/// </summary>
[Tool]
public abstract partial class ExtrudeVolumePackedVector2ArrayMesh : ExtrudePackedVector2ArrayMesh
{
    /* Private variables */
    protected float Thickness = 0.5f;
    protected Vector2 ThicknessVector => Thickness.ToVector2() / 2;
    protected bool RenderTop = true;
    protected bool RenderBottom = true;
    protected bool RenderEnds = true;

    /* Public variables */
    protected Vector2? JoinStart = null;
    protected Vector2? JoinEnd = null;

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

    /* Private methods */

    protected VolumePolyline GetPolyline() => new VolumePolyline
    {
        Points = Points,
        Thickness = Thickness,
        Direction = Direction,
        Length = Length,
        RenderTop = RenderTop,
        RenderBottom = RenderBottom,
        RenderEnds = RenderEnds,
        JoinStart = JoinStart,
        JoinEnd = JoinEnd
    };

    protected override IMeshComponent[] _GetComponents() => [GetPolyline()];

}
