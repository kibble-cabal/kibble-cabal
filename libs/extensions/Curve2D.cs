using Godot;

public static class Curve2DExtensions
{
    public static Vector2[] GetPointPositions(this Curve2D curve)
    {
        Vector2[] points = new Vector2[curve.PointCount];
        for (int i = 0; i < curve.PointCount; i++)
            points[i] = curve.GetPointPosition(i);
        return points;
    }

    public static Vector2 ClosestPoint(this Curve2D curve, Vector2 position) => curve.GetPointPositions().Closest(position);

    public static Vector2 ClosestPointOnSurface(this Curve2D curve, Vector2 position)
    {
        if (curve.PointCount == 0) return Vector2.Inf;
        float offset = curve.GetClosestOffset(position);
        return curve.Samplef(offset);
    }
}