using Godot;

public struct Quad2D : IMeshComponent
{
    public bool Invert { get; set; }
    public int Surface { get; set; }

    public Vector2 TopLeft;
    public Vector2 TopRight;
    public Vector2 BottomLeft;
    public Vector2 BottomRight;
    public float OffsetFromLineStart;
    public Vector3.Axis ProjectionAxis;
    public Vector3 ExtrudeDirection;
    public float ExtrudeAmount;

    public readonly Vector2 Direction => TopLeft.DirectionTo(BottomLeft);
    public readonly float ExtrudeLength => (ExtrudeDirection * ExtrudeAmount).Length().Abs();
    public readonly float Length => TopLeft.DistanceTo(BottomLeft).Abs();

    private readonly Vector2[] GetUVs()
    {
        var origin = new Vector2(OffsetFromLineStart, 0);
        var end = new Vector2(Length, ExtrudeLength);
        var tl = origin + end * new Vector2(0, 0);
        var tr = origin + end * new Vector2(1, 0);
        var br = origin + end * new Vector2(1, 1);
        var bl = origin + end * new Vector2(0, 1);
        return [br, tl, tr, tl, br, bl];
    }

    public Triangle[] GetTriangles()
    {
        var points = (
            TopRight: TopRight.ToVector3(ProjectionAxis) + ExtrudeDirection * ExtrudeAmount,
            BottomRight: BottomRight.ToVector3(ProjectionAxis) + ExtrudeDirection * ExtrudeAmount,
            TopLeft: TopLeft.ToVector3(ProjectionAxis),
            BottomLeft: BottomLeft.ToVector3(ProjectionAxis)
        );
        var uvs = GetUVs();
        var triangleA = new Triangle(points.TopLeft, points.BottomRight, points.TopRight, customUVs: (uvs[0], uvs[1], uvs[2]), inverted: Invert, surface: Surface);
        var triangleB = new Triangle(points.BottomRight, points.TopLeft, points.BottomLeft, customUVs: (uvs[3], uvs[4], uvs[5]), inverted: Invert, surface: Surface);
        return [triangleA, triangleB];
    }

    public static void Join(ref Quad2D a, ref Quad2D b)
    {
        if (a.TopRight.IsEqualApprox(b.BottomRight) && a.TopLeft.IsEqualApprox(b.BottomLeft)) return;
        var intersectionA = b.TopRight.Intersect(b.Direction, a.BottomRight, a.Direction, 5);
        var intersectionB = b.TopLeft.Intersect(b.Direction, a.BottomLeft, a.Direction, 5);
        b.TopRight = intersectionA;
        a.BottomRight = intersectionA;
        b.TopLeft = intersectionB;
        a.BottomLeft = intersectionB;
    }

    public static void Join(ref Quad2D[] quads, bool isClosed)
    {
        for (int i = 0; i < quads.Length - 1; i++)
            Join(ref quads[i], ref quads[i + 1]);
        if (isClosed && quads.Length >= 2)
            Join(ref quads[^1], ref quads[0]);
    }
}