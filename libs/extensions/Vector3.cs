using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class Vector3Extensions
{
    public static Vector3 ToVector3(this float f) => new(f, f, f);
    public static Vector2 ToVector2(this Vector3 vector, Vector3.Axis zeroAxis = Vector3.Axis.Y)
    {
        switch (zeroAxis)
        {
            case Vector3.Axis.X: return new Vector2(vector.Y, vector.Z);
            case Vector3.Axis.Z: return new Vector2(vector.X, vector.Y);
            case Vector3.Axis.Y:
            default:
                return new Vector2(vector.X, vector.Z);
        }
    }

    public static string ToPrecisionString(this Vector3 point, int precision = 2) => $"({point.X.ToPrecisionString(precision)}, {point.Y.ToPrecisionString(precision)}, {point.Z.ToPrecisionString(precision)})";

    /// <summary>
    /// Clamps the return value of Geometry3D.GetTriangleBarycentricCoords to only return points within the triangle.
    /// Taken from <a href="https://stackoverflow.com/questions/14467296/barycentric-coordinate-clamping-on-3d-triangle">this StackOverflow answer</a>.
    /// </summary>
    public static Vector3 ClampBarycentricCoords(this Vector3 coords, Vector3 p0, Vector3 p1, Vector3 p2)
    {
        static float clamp01(float val) => Mathf.Clamp(val, 0, 1);
        var p = p0 * coords.X + p1 * coords.Y + p2 * coords.Z;
        if (coords.X < 0)
        {
            var t = clamp01((p - p1).Dot(p2 - p1) / (p2 - p1).Dot(p2 - p1));
            return new Vector3(0, 1 - t, t);
        }
        if (coords.Y < 0)
        {
            var t = clamp01((p - p2).Dot(p0 - p2) / (p0 - p2).Dot(p0 - p2));
            return new Vector3(t, 0, 1 - t);
        }
        if (coords.Z < 0)
        {
            var t = clamp01((p - p0).Dot(p1 - p0) / (p1 - p0).Dot(p1 - p0));
            return new Vector3(1 - t, t, 0);
        }
        return coords;
    }


    public static Vector3 Closest(this Vector3 toPoint, Vector3 a, Vector3 b) => a.DistanceTo(toPoint).Abs() < b.DistanceTo(toPoint).Abs() ? a : b;

    public static Vector3 Closest(this Vector3[] vectors, Vector3 toPoint)
    {
        if (vectors.Length == 0) return Vector3.Inf;
        return vectors.OrderBy(vector => vector.DistanceTo(toPoint).Abs()).First();
    }

    public static IEnumerable<Vector3> Transformed(this IEnumerable<Vector3> vectors, Transform3D transform) => vectors.Select(vec => vec * transform);
}