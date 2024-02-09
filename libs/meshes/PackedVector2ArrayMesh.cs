using System.Drawing;
using Godot;

#nullable enable


/// <summary>
/// Base class for generating meshes based on sets of points.
/// </summary>
[Tool]
public partial class PackedVector2ArrayMesh : ArrayMesh
{
    /* Private variables */
    internal ProceduralMesh InternalMesh;
    protected Vector2[] Points = [];

    [Export]
    public bool flip
    {
        get => InternalMesh.FlipFaces;
        set
        {
            InternalMesh.FlipFaces = value;
            InternalMesh.Generate(this);
        }
    }

    [Export]
    public bool smooth_normals
    {
        get => InternalMesh.SmoothNormals;
        set
        {
            InternalMesh.SmoothNormals = value;
            InternalMesh.Generate(this);
        }
    }

    [Export]
    public Transform3D custom_transform
    {
        get => InternalMesh.CustomTransform;
        set
        {
            InternalMesh.CustomTransform = value;
            InternalMesh.Generate(this);
        }
    }

    internal Triangle[] GetTriangles()
    {
        if (!_BakePoints()) return [];
        return _GetTriangles();
    }

    internal virtual bool _BakePoints() => true;
    internal virtual Triangle[] _GetTriangles() => [];

    /* Public Methods */

    public PackedVector2ArrayMesh()
    {
        this.InternalMesh = new(GetTriangles, this);
    }
}

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

/// <summary>
/// Base class for generating polygon meshes based on sets of points.
/// </summary>
[Tool]
public partial class PolygonMeshBase : PackedVector2ArrayMesh
{
    /* Private variables */
    internal BaseMaterial3D? Material;

    /* Public variables */

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

    internal override bool _BakePoints() => Points.Length >= 2;

    internal override Triangle[] _GetTriangles()
    {
        var indices = Geometry2D.TriangulatePolygon(Points);
        Triangle[] triangles = new Triangle[indices.Length / 3];
        for (int i = 0; i < indices.Length - 2; i += 3)
        {
            var triangle = new Triangle(
                Points[indices[i]].ToVector3(),
                Points[indices[i + 1]].ToVector3(),
                Points[indices[i + 2]].ToVector3(),
                null,
                (
                    Points[indices[i]],
                    Points[indices[i + 1]],
                    Points[indices[i + 2]]
                )
            );
            triangles[i / 3] = triangle;
        }
        return triangles;
    }
}

/// <summary>
/// Base class for generating polyline meshes based on sets of points.
/// </summary>
[Tool]
public partial class PolylineMeshBase : PolygonMeshBase
{
    protected float Thickness = 0.1f;

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

    internal bool IsClosed() => Points.Length >= 3 && Points[^1].IsEqualApprox(Points[0]);

    internal Quad[] GetQuads()
    {
        var quads = new Quad[Points.Length - 1];
        var thickness = new Vector2(Thickness, Thickness) / 2;
        for (int i = 0; i < Points.Length - 1; i++)
        {
            var point = Points[i];
            var nextPoint = Points[i + 1];
            var angle = point.AngleToPoint(nextPoint);
            var quad = new Quad([
                point - thickness.Rotated(angle),
                nextPoint - thickness.Rotated(angle),
                point + thickness.Rotated(angle),
                nextPoint + thickness.Rotated(angle)
            ]);
            var dir = point.DirectionTo(nextPoint);

            // Account for angle at start
            if (i == 0)
                quad.Points[2] = quad.Points[2].MoveAway(quad.Points[3], Thickness);

            // Account for angle between points
            if (i > 0)
            {
                var prevDir = Points[i - 1].DirectionTo(point);
                var intersectionA = quad.Points[0].Intersect(dir, quads[i - 1].Points[1], prevDir);
                var intersectionB = quad.Points[2].Intersect(dir, quads[i - 1].Points[3], prevDir);
                quad.Points[0] = intersectionA;
                quads[i - 1].Points[1] = intersectionA;
                quad.Points[2] = intersectionB;
                quads[i - 1].Points[3] = intersectionB;
            }

            // Account for angle at end
            if (i == Points.Length - 2)
                quad.Points[1] = quad.Points[1].MoveAway(quad.Points[0], Thickness);

            quads[i] = quad;
        }
        // Account for angle at end
        if (IsClosed())
        {
            var lastQuad = quads[quads.Length - 1];
            var firstQuad = quads[0];
            var firstDir = Points[1].DirectionTo(Points[0]);
            var lastDir = Points[Points.Length - 2].DirectionTo(Points[Points.Length - 1]);
            var intersectionA = lastQuad.Points[0].Intersect(lastDir, firstQuad.Points[1], firstDir);
            var intersectionB = lastQuad.Points[2].Intersect(lastDir, firstQuad.Points[3], firstDir);
            lastQuad.Points[1] = intersectionA;
            firstQuad.Points[0] = intersectionA;
            lastQuad.Points[3] = intersectionB;
            firstQuad.Points[2] = intersectionB;
            quads[quads.Length - 1] = lastQuad;
            quads[0] = firstQuad;
        }
        return quads;
    }

    internal override Triangle[] _GetTriangles()
    {
        var quads = GetQuads();
        Triangle[] triangles = new Triangle[Points.Length * 2];
        for (int i = 0; i < quads.Length; i++)
        {
            if (quads[i].Points == null) continue;
            var (triangleA, triangleB) = quads[i].GetTriangles();
            triangles[i * 2] = triangleA;
            triangles[i * 2 + 1] = triangleB;
        }
        return triangles;
    }
}
