using Godot;

#nullable enable

/// <summary>
/// Base class for generating extruded meshes based on sets of points.
/// </summary>
[Tool]
public abstract partial class ExtrudeVolumePackedVector2ArrayMesh : ExtrudePackedVector2ArrayMesh
{
    /* Private variables */
    internal float Thickness = 0.5f;
    internal Vector2 ThicknessVector => Thickness.ToVector2() / 2;
    internal bool RenderTop = true;
    internal bool RenderBottom = true;
    internal bool RenderEnds = true;

    /* Public variables */

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

    /* Private methods */

    internal VolumePolyline GetPolyline() => new VolumePolyline
    {
        Points = Points,
        Thickness = Thickness,
        Direction = Direction,
        Length = Length,
        RenderTop = RenderTop,
        RenderBottom = RenderBottom,
        RenderEnds = RenderEnds
    };

    internal override Triangle[] _GetTriangles() => GetPolyline().GetTriangles();
    internal override int[] _GetSurfaces() => GetPolyline().GetSurfaces();
}
