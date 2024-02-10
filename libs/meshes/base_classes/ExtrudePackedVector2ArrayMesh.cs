using Godot;

#nullable enable

/// <summary>
/// Base class for generating extruded meshes based on sets of points.
/// </summary>
[Tool]
public abstract partial class ExtrudePackedVector2ArrayMesh : PackedVector2ArrayMesh
{
    /* Private variables */
    protected Vector3 Direction = Vector3.Up;
    protected float Length = 1.0f;
    protected BaseMaterial3D[] Materials = [];

    /* Public variables */

    [Export]
    public Vector3 direction
    {
        get => Direction;
        set
        {
            Direction = value;
            InternalMesh.Generate(this);
        }
    }

    [Export]
    public float length
    {
        get => Length;
        set
        {
            Length = value;
            InternalMesh.Generate(this);
        }
    }

    [Export]
    public BaseMaterial3D[] material
    {
        get => Materials;
        set
        {
            Materials = value;
            InternalMesh.OverrideMaterials = Materials;
            InternalMesh.Generate(this);
        }
    }

    /* Private methods */

    internal Segment GetSegment(Vector2 a, Vector2 b, float offsetFromStart) => new Segment(
        (a, b),
         Direction,
         Length,
         offsetFromStart
    );

    internal override Triangle[] _GetTriangles()
    {
        Triangle[] triangles = new Triangle[Points.Length * 2];
        float offset = 0;
        for (int i = 0; i < Points.Length - 1; i++)
        {
            var segment = GetSegment(Points[i], Points[i + 1], offset);
            var segmentTriangles = segment.GetTriangles();
            triangles[i * 2] = segmentTriangles[0];
            triangles[i * 2 + 1] = segmentTriangles[1];
            offset += Points[i].DistanceTo(Points[i + 1]);
        }
        return triangles;
    }

    internal override int[] _GetSurfaces() => [];
}
