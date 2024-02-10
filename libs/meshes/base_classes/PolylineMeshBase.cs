using Godot;

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
            var quad = new Quad
            {
                TopRight = point - thickness.Rotated(angle),
                BottomRight = nextPoint - thickness.Rotated(angle),
                TopLeft = point + thickness.Rotated(angle),
                BottomLeft = nextPoint + thickness.Rotated(angle)
            };

            // Account for angle at start
            if (i == 0)
                quad.TopLeft = quad.TopLeft.MoveAway(quad.BottomLeft, Thickness);

            // Account for angle between points
            if (i > 0)
            {
                var (previous, current) = quad.Joined(quads[i - 1]);
                quad = current;
                quads[i - 1] = previous;
            }

            // Account for angle at end
            if (i == Points.Length - 2)
                quad.BottomRight = quad.BottomRight.MoveAway(quad.TopRight, Thickness);

            quads[i] = quad;
        }
        // Account for angle at end
        if (IsClosed())
        {
            var (first, last) = quads[0].Joined(quads[quads.Length - 1]);
            quads[quads.Length - 1] = last;
            quads[0] = first;
        }
        return quads;
    }

    internal override Triangle[] _GetTriangles()
    {
        var quads = GetQuads();
        Triangle[] triangles = new Triangle[Points.Length * 2];
        for (int i = 0; i < quads.Length; i++)
        {
            var (triangleA, triangleB) = quads[i].GetTriangles();
            triangles[i * 2] = triangleA;
            triangles[i * 2 + 1] = triangleB;
        }
        return triangles;
    }
}
