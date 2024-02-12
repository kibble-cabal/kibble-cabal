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

    /* Private methods */

    protected Polygon GetPolygon() => new Polygon { Points = Points, Invert = Invert, ProjectionAxis = ProjectionAxis };

    protected override Vector2[] _BakePoints() => Points;
    protected override IMeshComponent[] _GetComponents() => [GetPolygon()];
}
