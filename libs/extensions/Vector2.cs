using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public class MiterLimitReachedException : Exception { }

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

    public static Vector2 Intersect(this Vector2 a, Vector2 aDirection, Vector2 b, Vector2 bDirection, float upperLimit, float lowerLimit)
    {
        var u = (a.Y * bDirection.X + bDirection.Y * b.X - b.Y * bDirection.X - bDirection.Y * a.X) / (aDirection.X * bDirection.Y - aDirection.Y * bDirection.X);
        if (u > upperLimit || u < lowerLimit)
            throw new MiterLimitReachedException { }; ;
        return a + aDirection * u;
    }

    public static Vector2 Intersect(this Vector2 a, Vector2 aDirection, Vector2 b, Vector2 bDirection, float limit = 1) => a.Intersect(aDirection, b, bDirection, limit, -limit);


    public static Vector2 MoveAway(this Vector2 a, Vector2 b, float amount)
    {
        var dir = a.DirectionTo(b);
        return a - dir * amount;
    }

    public static Vector2[] OffsetBy(this Vector2[] points, float amount, bool isClosed)
    {
        if (points.Length <= 2) return points;

        Vector2[] newPoints = new Vector2[points.Length - 1];

        // Expand each line segment by the given amount
        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2 p1 = points[i], p2 = points[i + 1];
            Vector2 rotation = amount.ToVector2().Rotated(p2.AngleToPoint(p1));
            newPoints[i] = p1 + rotation;
        }

        if (isClosed)
            newPoints = [.. newPoints, newPoints[0]];

        return newPoints;
    }

    public static string ToPrecisionString(this Vector2 point, int precision = 2) => $"({point.X.ToPrecisionString(precision)}, {point.Y.ToPrecisionString(precision)})";

    public static Vector2 Midpoint(this Vector2 p1, Vector2 p2) => p1 + (p2 - p1) / 2;

    public static Vector2 GetNormal(this Vector2 prev, Vector2 next) => new Vector2(-(next.Y - prev.Y), next.X - prev.X).Normalized();

    public static Vector2 Average(this IEnumerable<Vector2> points)
    {
        var p = points.Where(point => point.IsFinite());
        if (p.Count() == 0) return Vector2.Inf;
        Vector2 avg = p.ElementAt(0);
        foreach (var point in points) avg = (avg + point) / 2;
        return avg;
    }

    public static Rect2 GetBoundingBox(this Vector2[] points)
    {
        if (points.Length == 0) return new();
        Rect2 rect = new(points[0], Vector2.Zero);
        foreach (var point in points) rect = rect.Expand(point);
        return rect;
    }

}