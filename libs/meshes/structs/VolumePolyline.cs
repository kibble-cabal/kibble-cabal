using System.Linq;
using Godot;

public struct VolumePolyline : IMeshComponent
{
    enum SurfaceIndex
    {
        Outer = 0,
        Inner = 1,
        Top = 2,
        Bottom = 3,
        End = 4
    }

    public bool Invert { get; set; }
    public int Surface { get; set; }
    public Vector2[] Points;
    public float Thickness;
    public Vector3 Direction;
    public float Length;
    public bool RenderTop;
    public bool RenderBottom;
    public bool RenderEnds;

    /// <summary>
    /// Optional. If provided, simulated joining to the given point, as if it were the first point provided.
    /// </summary>
    public Vector2? JoinStart;
    /// <summary>
    /// Optional. If provided, simulated joining to the given point, as if were the last point provided.
    /// </summary>
    public Vector2? JoinEnd;

    public readonly Vector2 ThicknessVector => Thickness.ToVector2() / 2;

    public readonly bool IsClosed() => Points.Length >= 3 && Points[0].IsEqualApprox(Points[^1]) && JoinStart == null && JoinEnd == null;

    private Segment GetSegment(Vector2 a, Vector2 b, float offsetFromStart, SurfaceIndex surface, bool inverted = false) => new Segment
    {
        Points = (a, b),
        Direction = Direction,
        Length = Length,
        Offset = offsetFromStart,
        Invert = inverted,
        Surface = (int)surface,
    };

    private (Segment[] Outer, Segment[] Inner) GetSegments()
    {
        var isClosed = IsClosed();
        Segment[] outerSegments = new Segment[Points.Length - 1];
        Segment[] innerSegments = new Segment[Points.Length - 1];
        float offset = 0;
        for (int i = 0; i < Points.Length - 1; i++)
        {
            Vector2 a = Points[i], b = Points[i + 1];
            var outerSegment = GetSegment(a, b, offset, SurfaceIndex.Outer).OffsetBy(Thickness / 2);
            var innerSegment = GetSegment(a, b, offset, SurfaceIndex.Inner, true).OffsetBy(-Thickness / 2);

            outerSegments[i] = outerSegment;
            innerSegments[i] = innerSegment;
            offset += a.DistanceTo(b);
        }
        outerSegments.Join(isClosed);
        innerSegments.Join(isClosed);

        if (Points.Length > 1 && !isClosed)
        {
            // Account for angle at start
            innerSegments[0].Points.A = innerSegments[0].Points.A.MoveToward(innerSegments[0].Points.B, Thickness);
            // Account for angle at end
            outerSegments[^1].Points.B = outerSegments[^1].Points.B.MoveToward(outerSegments[^1].Points.A, Thickness);

            // TODO: Join Code
            // if (JoinStart is Vector2 prevPoint)
            // {
            //     var outerJoinSegment = GetSegment(prevPoint, Points[0], 0, SurfaceIndex.Outer);
            //     outerSegments[0] = outerSegments[0].SimulateJoinedStart(outerJoinSegment);
            // }

        }

        return (outerSegments, innerSegments);
    }

    private Polyline GetPolyline(Vector3 offset, SurfaceIndex surface, bool inverted = false) => new()
    {
        Points = Points,
        Thickness = Thickness,
        ZeroAxis = Vector3.Axis.Y,
        Offset = offset,
        JoinStart = JoinStart,
        JoinEnd = JoinEnd,
        Invert = inverted,
        Surface = (int)surface
    };

    private readonly Triangle[] GetSideTriangles(Vector2 a, Vector2 b, bool inverted)
    {
        Triangle[] triangles = new Triangle[2];
        var lengthVector = new Vector3(0, Length, 0);
        var bl = a.ToVector3();
        var br = b.ToVector3();
        var tl = bl + lengthVector;
        var tr = br + lengthVector;
        triangles[0] = new Triangle(tr, br, tl, inverted: inverted, surface: (int)SurfaceIndex.End);
        triangles[1] = new Triangle(bl, tl, br, inverted: inverted, surface: (int)SurfaceIndex.End);
        return triangles;
    }

    public IMeshComponent[] GetComponents()
    {
        var (outerSegments, innerSegments) = GetSegments();
        IMeshComponent[] components = [
            .. outerSegments,
            .. innerSegments
        ];
        if (RenderTop) components = [.. components, GetPolyline(new Vector3(0, Length, 0), SurfaceIndex.Top)];
        if (RenderBottom) components = [.. components, GetPolyline(Vector3.Zero, SurfaceIndex.Bottom, true)];
        if (RenderEnds && !IsClosed())
            components = [
                .. components,
                .. GetSideTriangles(outerSegments[0].Points.A, innerSegments[0].Points.A, true),
                .. GetSideTriangles(outerSegments[^1].Points.B, innerSegments[^1].Points.B, false)
            ];
        return components;
    }

    public Triangle[] GetTriangles() => GetComponents().SelectMany(component => component.GetTriangles()).ToArray();
}
