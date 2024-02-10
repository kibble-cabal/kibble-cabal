using System;
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

    public static Vector2 Intersect(this Vector2 a, Vector2 aDirection, Vector2 b, Vector2 bDirection, float limit = -1)
    {
        var u = (a.Y * bDirection.X + bDirection.Y * b.X - b.Y * bDirection.X - bDirection.Y * a.X) / (aDirection.X * bDirection.Y - aDirection.Y * bDirection.X);
        if (limit >= 0 && Mathf.Abs(u) > limit)
            return a + aDirection * Mathf.Min(u, limit * Mathf.Sign(u));
        if (Mathf.Abs(u) > 0.00001f)
            return a + aDirection * u;
        return a;
    }

    public static Vector2 MoveAway(this Vector2 a, Vector2 b, float amount)
    {
        var dir = a.DirectionTo(b);
        return a - dir * amount;
    }
}