using Godot;

#nullable enable

/// <summary>
/// Base class for generating extruded meshes based on sets of points.
/// </summary>
[Tool]
public partial class ExtrudePackedVector2ArrayMesh : PackedVector2ArrayMesh
{
    /* Private variables */
    protected Vector3 Direction = Vector3.Up;
    protected float Length = 1.0f;
    protected BaseMaterial3D? Material;

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
    public BaseMaterial3D? material
    {
        get => Material;
        set
        {
            Material = value;
            if (Material is Material mat)
                InternalMesh.OverrideMaterials = [mat];
            InternalMesh.Generate(this);
        }
    }

    /* Private methods */

    internal Segment GetSegment(Vector2 a, Vector2 b, float offset) => new Segment(
        (new Vector3(a.X, 0, a.Y), new Vector3(b.X, 0, b.Y)),
         Direction,
         Length,
         offset
    );

    internal override Triangle[] _GetTriangles()
    {
        Triangle[] triangles = new Triangle[Points.Length * 2];
        float offset = 0;
        for (int i = 0; i < Points.Length - 1; i++)
        {
            var segment = GetSegment(Points[i], Points[i + 1], offset);
            var (triangleA, triangleB) = segment.GetTriangles();
            triangles[i * 2] = triangleA;
            triangles[i * 2 + 1] = triangleB;
            offset += Points[i].DistanceTo(Points[i + 1]);
        }
        return triangles;
    }
}
