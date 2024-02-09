using Godot;


public static class Vector2Extensions
{
    public static Vector3 ToVector3(this Vector2 vector) => new Vector3(vector.X, 0, vector.Y);
    public static Vector2 FromVector3(this Vector3 vector) => new Vector2(vector.X, vector.Z);

}