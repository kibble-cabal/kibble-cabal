using Godot;


public class F
{
    public const float AlmostZero = 0.001f;
}

public static class FloatExtensions
{
    public static float Abs(this float value) => Mathf.Abs(value);
}