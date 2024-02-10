using Godot;

using Segment2D = (Godot.Vector2 A, Godot.Vector2 B);

public struct Segment : IMeshComponent
{
    public Segment2D Points;
    public float Offset;
    public Vector3 Direction;
    public float Length;
    public Vector3.Axis ZeroAxis = Vector3.Axis.Y;
    public readonly float SegmentLength => Points.A.DistanceTo(Points.B);
    public readonly float Angle => Points.A.AngleTo(Points.B);
    public readonly Vector2 SegmentDirection => Points.A.DirectionTo(Points.B);

    internal Vector2[] GetUVs()
    {
        var o = new Vector2(Offset, 0);
        var e = new Vector2(SegmentLength, Direction.Length() * Length);
        var tl = o + e * new Vector2(0, 0);
        var tr = o + e * new Vector2(1, 0);
        var br = o + e * new Vector2(1, 1);
        var bl = o + e * new Vector2(0, 1);
        return [tl, br, bl, br, tl, tr];
    }

    public Segment(Segment2D points, Vector3 direction, float length, float offset)
    {
        this.Points = points;
        this.Direction = direction;
        this.Length = length;
        this.Offset = offset;
    }

    public (Segment Current, Segment Next) Joined(Segment next)
    {
        var intersection = Points.B.Intersect(SegmentDirection, next.Points.A, next.SegmentDirection);
        Points.B = intersection;
        next.Points.A = intersection;
        return (this, next);
    }

    public Segment OffsetBy(float byAmount)
    {
        var amountVector = byAmount.ToVector2().Rotated(Points.A.AngleToPoint(Points.B));
        Points.A += amountVector;
        Points.B += amountVector;
        return this;
    }

    public int GetTriangleCount() => 2;
    public Triangle[] GetTriangles()
    {
        var points = (
            A: Points.A.ToVector3(ZeroAxis),
            B: Points.B.ToVector3(ZeroAxis),
            C: Points.A.ToVector3(ZeroAxis) + Direction * Length,
            D: Points.B.ToVector3(ZeroAxis) + Direction * Length
        );
        var uvs = GetUVs();
        var triangleA = new Triangle(points.C, points.B, points.A);
        var triangleB = new Triangle(points.B, points.C, points.D);
        triangleA.CustomUVs = (uvs[0], uvs[1], uvs[2]);
        triangleB.CustomUVs = (uvs[3], uvs[4], uvs[5]);
        return [triangleA, triangleB];
    }
}

public struct VolumizedSegment
{
    public Segment Base;
    public Segment Inner;
    public Segment Outer;
    public float Thickness;

    public VolumizedSegment(Segment baseSegment, float thickness)
    {
        this.Base = baseSegment;
        this.Outer = baseSegment.OffsetBy(thickness / 2);
        this.Inner = baseSegment.OffsetBy(-thickness / 2);
    }
}

public static class SegmentExtension
{
    public static void Join(this Segment[] segments, bool isClosed)
    {
        for (int i = 0; i < segments.Length - 1; i++)
        {
            var (curr, next) = segments[i].Joined(segments[i + 1]);
            segments[i] = curr;
            segments[i + 1] = next;
        }
        if (segments.Length >= 3 && isClosed)
        {
            var (curr, next) = segments[^1].Joined(segments[0]);
            segments[0] = next;
            segments[^1] = curr;
        }
    }
}