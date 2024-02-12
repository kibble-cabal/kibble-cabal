using Godot;


public static class Vector2Extensions
{
    public static Vector2 ToVector2(this float f) => new(f, f);

    public static Vector3 ToVector3(this Vector2 vector, Vector3.Axis zeroAxis = Vector3.Axis.Y, float zeroValue = 0)
    {
        switch (zeroAxis)
        {
            case Vector3.Axis.X:
                return new Vector3(zeroValue, vector.X, vector.Y);
            case Vector3.Axis.Z:
                return new Vector3(vector.X, vector.Y, zeroValue);
            case Vector3.Axis.Y:
            default:
                return new Vector3(vector.X, zeroValue, vector.Y);
        }
    }
    public static Vector2 FromVector3(this Vector3 vector) => new Vector2(vector.X, vector.Z);

    public static Vector2 Intersect(this Vector2 a, Vector2 aDirection, Vector2 b, Vector2 bDirection, float limit = 5)
    {
        var u = (a.Y * bDirection.X + bDirection.Y * b.X - b.Y * bDirection.X - bDirection.Y * a.X) / (aDirection.X * bDirection.Y - aDirection.Y * bDirection.X);
        if (limit >= 0 && u.Abs() > limit)
            return a + aDirection * limit * u.Sign();
        if (u.Abs() > F.AlmostZero)
            return a + aDirection * u;
        return a;
    }

    public static Vector2 MoveAway(this Vector2 a, Vector2 b, float amount)
    {
        var dir = a.DirectionTo(b);
        return a - dir * amount;
    }

    public static Vector2[] OffsetBy(this Vector2[] points, float amount, bool isClosed)
    {
        if (points.Length <= 2) return points;

        (Vector2 A, Vector2 B)[] segments = new (Vector2 A, Vector2 B)[points.Length];

        // Expand each line segment by the given amount
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2 p1 = points[i], p2 = points[i + 1];
            Vector2 rotation = amount.ToVector2().Rotated(p2.AngleToPoint(p1));
            segments[i] = (p1 + rotation, p2 + rotation);
        }
        segments[^1] = (points[^1], points[^1]);

        // Find the new points by finding intersections between expanded points
        Vector2[] newPoints = new Vector2[segments.Length];
        newPoints[0] = segments[0].A;
        for (int i = 0; i < segments.Length - 1; i++)
        {
            var (a1, a2) = segments[i];
            var (b1, b2) = segments[i + 1];
            var aDir = a1.DirectionTo(a2);
            var bDir = b1.DirectionTo(b2);
            var intersection = a2.Intersect(aDir, b1, bDir);
            newPoints[i + 1] = intersection;
        }

        // Handle closed shapes
        if (isClosed)
        {
            var aDir = newPoints[0].DirectionTo(newPoints[1]);
            var bDir = newPoints[^1].DirectionTo(newPoints[^2]);
            var intersection = newPoints[0].Intersect(aDir, newPoints[^1], bDir);
            newPoints[0] = intersection;
            newPoints[^1] = intersection;
        }

        return newPoints;
    }
}