using Godot;

public static class Vector3Extensions
{
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

}