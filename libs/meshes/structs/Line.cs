using System.Linq;
using Godot;

#nullable enable

public struct Line : IMeshComponent
{
    public bool Invert { get; set; }
    public int Surface { get; set; }
    public Vector2[] Points;
    public bool Flat;
    public Vector3.Axis ProjectionAxis;
    public Vector3 ExtrudeDirection;
    public float ExtrudeAmount;
    public Transform3D CustomTransform;
    // public int[]? Surfaces;

    public Vector2? JoinStart;
    public Vector2? JoinEnd;

    public bool IsClosed() => Points.Length >= 3 && Points[0].IsEqualApprox(Points[^1]);

    public Line OffsetBy(float amount)
    {
        Points = Points.OffsetBy(amount, IsClosed());
        return this;
    }

    public Vector2[] GetFlatInnerPolygon()
    {
        var quads = GetQuads();
        var points = quads.Select(quad => quad.TopLeft);
        if (quads.Length > 0)
            return [.. points, quads[^1].BottomLeft];
        return [.. points];
    }

    public Vector2[] GetFlatOuterPolygon()
    {
        var quads = GetQuads();
        var points = quads.Select(quad => quad.TopRight);
        if (quads.Length > 0)
            return [.. points, quads[^1].BottomRight];
        return [.. points];
    }

    public Quad2D GetQuad(Vector2 p1, Vector2 p2, float offsetFromLineStart)
    {
        if (Flat)
        {
            var amount = new Vector2(0, ExtrudeAmount) / 2;
            Vector2 rotation = amount.Rotated(p1.AngleToPoint(p2));
            return new Quad2D
            {
                TopLeft = p1 - rotation,
                TopRight = p1 + rotation,
                BottomLeft = p2 - rotation,
                BottomRight = p2 + rotation,
                ProjectionAxis = ProjectionAxis,
                ExtrudeAmount = 0,
                ExtrudeDirection = Vector3.Zero,
                OffsetFromLineStart = offsetFromLineStart,
                Invert = Invert,
                Surface = Surface
            };
        }
        else return new Quad2D
        {
            TopLeft = p1,
            TopRight = p1,
            BottomLeft = p2,
            BottomRight = p2,
            ProjectionAxis = ProjectionAxis,
            ExtrudeAmount = ExtrudeAmount,
            ExtrudeDirection = ExtrudeDirection,
            OffsetFromLineStart = offsetFromLineStart,
            Invert = Invert,
            Surface = Surface
        };
    }

    public Vector2[] GetPoints()
    {
        var points = Points;
        if (JoinStart is Vector2 joinStart) points = [joinStart, .. points];
        if (JoinEnd is Vector2 joinEnd) points = [.. points, joinEnd];
        return points;
    }

    public Quad2D[] GetQuads()
    {
        var points = Points;
        if (points.Length <= 1) return [];
        Quad2D[] quads = new Quad2D[points.Length - 1];
        float offsetFromLineStart = 0.0f;
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2 p1 = points[i], p2 = points[i + 1];
            quads[i] = GetQuad(p1, p2, offsetFromLineStart);
            offsetFromLineStart += quads[i].Length;
        }

        Quad2D.Joined(ref quads, IsClosed());

        // if (quads.Length > 0 && JoinStart != null)
        //     quads = quads.Skip(1).ToArray();

        // if (quads.Length > 0 && JoinEnd != null)
        //     quads = quads.SkipLast(1).ToArray();

        if (quads.Length > 0 && JoinStart is Vector2 joinStart)
            Quad2D.SimulateJoinStart(ref quads, GetQuad(joinStart, points[0], 0));

        if (quads.Length > 0 && JoinEnd is Vector2 joinEnd)
            Quad2D.SimulateJoinEnd(ref quads, GetQuad(joinEnd, points[^1], 0));

        return quads;
    }

    public Triangle[] GetTriangles()
    {
        Quad2D[] quads = GetQuads();
        if (quads.Length == 0) return [];
        Triangle[] triangles = new Triangle[quads.Length * 2];
        for (int i = 0; i < quads.Length; i++)
        {
            var tris = quads[i].GetTriangles();
            triangles[i * 2] = tris[0] * CustomTransform.AffineInverse();
            triangles[i * 2 + 1] = tris[1] * CustomTransform.AffineInverse();
            // if (Surfaces is int[] surfaces)
            // {
            //     triangles[i * 2].Surface = surfaces[i];
            //     triangles[i * 2 + 1].Surface = surfaces[i];
            // }
        }
        return triangles;
    }
}