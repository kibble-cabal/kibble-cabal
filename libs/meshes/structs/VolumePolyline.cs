using Godot;

public struct VolumePolyline : IMeshComponent
{
    public Vector2[] Points;
    public float Thickness;
    public Vector3 Direction;
    public float Length;
    public bool RenderTop;
    public bool RenderBottom;
    public bool RenderEnds;

    public Vector2 ThicknessVector => Thickness.ToVector2() / 2;

    public bool IsClosed() => Points.Length >= 3 && Points[0].IsEqualApprox(Points[^1]);

    internal Segment GetSegment(Vector2 a, Vector2 b, float offsetFromStart) => new Segment(
        (a, b),
        Direction,
        Length,
        offsetFromStart
    );

    internal (Segment[] Outer, Segment[] Inner) GetSegments()
    {
        var isClosed = IsClosed();
        Segment[] outerSegments = new Segment[Points.Length - 1];
        Segment[] innerSegments = new Segment[Points.Length - 1];
        float offset = 0;
        for (int i = 0; i < Points.Length - 1; i++)
        {
            Vector2 a = Points[i], b = Points[i + 1];
            var outerSegment = GetSegment(a, b, offset).OffsetBy(Thickness / 2);
            var innerSegment = GetSegment(a, b, offset).OffsetBy(-Thickness / 2);

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
        }

        return (outerSegments, innerSegments);
    }

    internal Polyline GetTopPolyline() => new Polyline
    {
        Points = Points,
        Thickness = Thickness,
        ZeroAxis = Vector3.Axis.Y,
        Offset = new Vector3(0, Length, 0)
    };

    internal Polyline GetBottomPolyline() => new Polyline
    {
        Points = Points,
        Thickness = Thickness,
        ZeroAxis = Vector3.Axis.Y
    };

    internal Triangle[] GetSideTriangles(Vector2 a, Vector2 b)
    {
        Triangle[] triangles = new Triangle[2];
        var lengthVector = new Vector3(0, Length, 0);
        var bl = a.ToVector3();
        var br = b.ToVector3();
        var tl = bl + lengthVector;
        var tr = br + lengthVector;
        triangles[0] = new Triangle(tr, br, tl);
        triangles[1] = new Triangle(bl, tl, br);
        return triangles;
    }

    public int GetTriangleCount()
    {
        int triangleCount = Points.Length * 4;
        if (RenderTop) triangleCount += Points.Length * 2;
        if (RenderBottom) triangleCount += Points.Length * 2;
        if (RenderEnds) triangleCount += 4;
        return triangleCount;
    }

    public Triangle[] GetTriangles()
    {
        var (outerSegments, innerSegments) = GetSegments();
        int triangleIndex = 0;
        Triangle[] triangles = new Triangle[Points.Length * 4];
        foreach (var outerSegment in outerSegments)
        {
            var segmentTriangles = outerSegment.GetTriangles();
            triangles[triangleIndex] = segmentTriangles[0];
            triangles[triangleIndex + 1] = segmentTriangles[1];
            triangleIndex += 2;
        }
        foreach (var innerSegment in innerSegments)
        {
            var segmentTriangles = innerSegment.GetTriangles();
            triangles[triangleIndex] = segmentTriangles[0].Inverted();
            triangles[triangleIndex + 1] = segmentTriangles[1].Inverted();
            triangleIndex += 2;
        }
        if (RenderTop) triangles = [.. triangles, .. GetTopPolyline().GetTriangles()];
        if (RenderBottom) triangles = [.. triangles, .. GetBottomPolyline().GetTriangles().Inverted()];
        if (RenderEnds && !IsClosed())
            triangles = [
                .. triangles,
                .. GetSideTriangles(outerSegments[0].Points.A, innerSegments[0].Points.A).Inverted(),
                .. GetSideTriangles(outerSegments[^1].Points.B, innerSegments[^1].Points.B)
            ];
        return triangles;
    }

    public int[] GetSurfaces()
    {
        int[] surfaces = [Points.Length * 2 - 2];
        if (RenderTop) surfaces = [.. surfaces, Points.Length * 4 - 2];
        if (RenderBottom) surfaces = [.. surfaces, Points.Length * 6 - 2];
        if (RenderEnds) surfaces = [.. surfaces, Points.Length * 8 - 2];
        return surfaces;
    }
}
