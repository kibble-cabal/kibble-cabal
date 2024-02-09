using Godot;
using Godot.Collections;

public static class PackedVector2ArrayExtensions
{
    public static Vector2 Closest(this Vector2[] vectors, Vector2 toPoint)
    {
        if (vectors.Length == 0) return Vector2.Inf;
        Vector2 closest = vectors[0];
        foreach (var currentPoint in vectors)
            closest = toPoint.Closest(currentPoint, closest);
        return closest;
    }

    public static Vector2 Closest(this Array<Vector2> vectors, Vector2 toPoint) => vectors.ToPackedArray().Closest(toPoint);

    public static Vector2[] Grow(this Vector2[] polygon, float amount, Geometry2D.PolyJoinType joinType = Geometry2D.PolyJoinType.Miter)
    {
        var points = Geometry2D.OffsetPolygon(polygon, amount, joinType);
        return points.Count > 0 ? points[0] : [];
    }
}