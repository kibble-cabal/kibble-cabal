using Godot;


public class F
{
    public const float AlmostZero = 0.001f;
}

public static class FloatExtensions
{
    public static float Abs(this float value) => Mathf.Abs(value);
    public static float Sign(this float value) => Mathf.Sign(value);
    public static float Fract(this float value) => value - Mathf.Round(value);
}