using Godot;


public static class Vector2Extensions
{
    public static Vector3 ToVector3(this Vector2 vector) => new Vector3(vector.X, 0, vector.Y);
    public static Vector2 FromVector3(this Vector3 vector) => new Vector2(vector.X, vector.Z);

    public static Vector2 Intersect(this Vector2 a, Vector2 aDirection, Vector2 b, Vector2 bDirection)
    {
        var u = (a.Y * bDirection.X + bDirection.Y * b.X - b.Y * bDirection.X - bDirection.Y * a.X) / (aDirection.X * bDirection.Y - aDirection.Y * bDirection.X);
        if (Mathf.Abs(u) > 0.0001f)
            return a + aDirection * u;
        return a;
    }

    public static Vector2 MoveAway(this Vector2 a, Vector2 b, float amount)
    {
        var dir = a.DirectionTo(b);
        return a - dir * amount;
    }
}