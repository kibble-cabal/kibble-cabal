using System;
using Godot;

using Godot.Collections;

using BakedPointsDictionary = System.Collections.Generic.SortedDictionary<float, Godot.Vector2>;
using BakedPoint = System.Collections.Generic.KeyValuePair<float, Godot.Vector2>;

public static class MathExtension
{
    public static Vector2 Closest(this Vector2 toPoint, Vector2 a, Vector2 b) => Mathf.Abs(a.DistanceTo(toPoint)) < Mathf.Abs(b.DistanceTo(toPoint)) ? a : b;

    /// <summary>
    /// If threshold is less than zero, returns snapped position. Otherwise, returns snapped position only if the snap distance is below the threshold.
    /// </summary>
    public static Vector2 Snap(this Vector2 position, Vector2 snapPoint, float threshold) => threshold < 0 || Mathf.Abs(position.DistanceTo(snapPoint)) < threshold ? snapPoint : position;
}

[GlobalClass]
public partial class Tessellator : RefCounted
{

    private static void Bake(
        ref BakedPointsDictionary baked,
        (Vector2 start, Vector2 end, Vector2 startHandle, Vector2 endHandle) points,
        float begin,
        float end,
        int depth,
        int maxDepth,
        float tolerance
    )
    {
        float mp = begin + (end - begin) * 0.5f;
        Vector2 interpolatedStart = points.start.BezierInterpolate(points.start + points.startHandle, points.end + points.endHandle, points.end, begin);
        Vector2 interpolatedMiddle = points.start.BezierInterpolate(points.start + points.startHandle, points.end + points.endHandle, points.end, mp);
        Vector2 interpolatedEnd = points.start.BezierInterpolate(points.start + points.startHandle, points.end + points.endHandle, points.end, end);

        Vector2 normalizedA = (interpolatedMiddle - interpolatedStart).Normalized();
        Vector2 normalizedB = (interpolatedEnd - interpolatedMiddle).Normalized();

        float dp = normalizedA.Dot(normalizedB);

        if (dp < Math.Cos(tolerance * (3.14159256 / 180.0f)))
            baked[mp] = interpolatedMiddle;

        if (depth < maxDepth)
        {
            Bake(ref baked, points, begin, mp, depth + 1, maxDepth, tolerance);
            Bake(ref baked, points, mp, end, depth + 1, maxDepth, tolerance);
        }
    }

    public static Vector2 ClosestPointToBezierCurve(
        Vector2 toPoint,
        Vector2 start,
        Vector2 end,
        Vector2 startHandle,
        Vector2 endHandle,
        float min = 0.0f,
        float max = 1.0f,
        float epsilon = 0.05f
    )
    {
        var increment = (max - min) / 2;
        var currentOffset = min + increment;
        var currentPoint = start.BezierInterpolate(start + startHandle, end + endHandle, end, min + increment);
        if (increment > epsilon)
        {
            var highPoint = ClosestPointToBezierCurve(toPoint, start, end, startHandle, endHandle, currentOffset, currentOffset + increment, epsilon);
            var lowPoint = ClosestPointToBezierCurve(toPoint, start, end, startHandle, endHandle, currentOffset - increment, currentOffset, epsilon);
            return toPoint.Closest(toPoint.Closest(highPoint, lowPoint), currentPoint);
        }
        return currentPoint;
    }

    public static Vector2 closest_point_to_bezier_curve(
        Vector2 to_point,
        Vector2 start,
        Vector2 end,
        Vector2 start_handle,
        Vector2 end_handle
    ) => ClosestPointToBezierCurve(to_point, start, end, start_handle, end_handle, 0.0f, 1.0f, 0.05f);

    public static Vector2 closest_point_to_bezier_curve(
        Vector2 to_point,
        Vector2 start,
        Vector2 end,
        Vector2 start_handle,
        Vector2 end_handle,
        float epsilon
    ) => ClosestPointToBezierCurve(to_point, start, end, start_handle, end_handle, 0.0f, 1.0f, epsilon);

    public static Vector2 closest_point_to_bezier_curve(
        Vector2 to_point,
        Vector2 start,
        Vector2 end,
        Vector2 start_handle,
        Vector2 end_handle,
        float min,
        float max,
        float epsilon
    ) => ClosestPointToBezierCurve(to_point, start, end, start_handle, end_handle, min, max, epsilon);

    public static Curve2D smoothed(Curve2D curve, int start_index, int end_index)
    {
        Curve2D smoothedCurve = (Curve2D)curve.Duplicate();
        if (curve.PointCount < start_index + 3 || curve.PointCount <= end_index) return smoothedCurve;

        for (int i = start_index + 2; i <= end_index; i++)
        {
            var p1 = curve.GetPointPosition(i - 2);
            var p2 = curve.GetPointPosition(i - 1);
            var p3 = curve.GetPointPosition(i);

            var dirTo1 = p2.DirectionTo(p1);
            var dirFrom3 = p3.DirectionTo(p2);
            var len = MathF.Min(Mathf.Abs(p2.DistanceTo(p1)) / 2.5f, Mathf.Abs(p2.DistanceTo(p1)) / 2.5f);
            var avgDir = (dirTo1 + dirFrom3) / 2;
            var inHandle = avgDir * len;
            var outHandle = inHandle.Rotated(MathF.PI);
            smoothedCurve.SetPointIn(i - 1, inHandle);
            smoothedCurve.SetPointOut(i - 1, outHandle);
        }

        return smoothedCurve;
    }

    public static Curve2D smoothed(Curve2D curve) => smoothed(curve, 0, curve.PointCount - 1);

    public static Curve2D simplified(Curve2D curve, float position_tolerance, float angle_tolerance)
    {
        Curve2D simplifiedCurve = new();
        if (curve.PointCount < 1) return simplifiedCurve;
        simplifiedCurve.AddPoint(curve.GetPointPosition(0), curve.GetPointIn(0), curve.GetPointOut(0));
        int skippedPoints = 0;
        for (int i = 1; i < curve.PointCount - 1; i += 1)
        {
            var simplifiedIndex = simplifiedCurve.PointCount - 1;
            var point = curve.GetPointPosition(i);
            var isPointClose = Mathf.Abs(simplifiedCurve.GetPointPosition(simplifiedIndex).DistanceTo(point)) <= position_tolerance;
            var isAngleClose = Mathf.Abs(simplifiedCurve.GetPointPosition(simplifiedIndex).AngleTo(point)) <= angle_tolerance;
            if (!isPointClose || !isAngleClose)
            {
                // Add an averaged point
                Vector2 inHandle = curve.GetPointIn(i);
                Vector2 outHandle = curve.GetPointOut(i);
                for (int skippedIndex = 0; skippedIndex < skippedPoints; skippedIndex++)
                    outHandle += point - curve.GetPointPosition(i - skippedIndex);
                outHandle /= skippedPoints;
                simplifiedCurve.AddPoint(point, inHandle, outHandle);
                skippedPoints = 0;
            }
            else skippedPoints += 1;
        }
        int last = curve.PointCount - 1;
        simplifiedCurve.AddPoint(curve.GetPointPosition(last), curve.GetPointIn(last), curve.GetPointOut(last));
        return simplifiedCurve;
    }

    public static Curve2D smoothed_simplified(Curve2D curve, float position_tolerance, float angle_tolerance) => smoothed(simplified(curve, position_tolerance, angle_tolerance));

    public static Vector2[] tessellate(
        Vector2 start,
        Vector2 end,
        Vector2 start_handle,
        Vector2 end_handle,
        int max_stages,
        float tolerance
    )
    {
        Array<Vector2> array = [];
        BakedPointsDictionary bakedMidpoints = [];

        Bake(ref bakedMidpoints, (start, end, start_handle, end_handle), 0, 1, 0, max_stages, tolerance);

        array.Add(start);
        foreach (BakedPoint midpoint in bakedMidpoints)
            array.Add(midpoint.Value);
        array.Add(end);

        Vector2[] value = new Vector2[array.Count];
        array.CopyTo(value, 0);
        return value;
    }

    public static Vector2[] tessellate(
        Vector2 start,
        Vector2 end,
        Vector2 start_handle,
        Vector2 end_handle
    ) => tessellate(start, end, start_handle, end_handle, 5, 4);

}