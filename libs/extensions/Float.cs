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
    public static string ToPrecisionString(this float value, int precision = 2) => $"{Mathf.Snapped(value, 1.0f / Mathf.Pow(10, precision))}".PadDecimals(2);
    public static bool IsEqualApprox(this float value, float other) => Mathf.IsEqualApprox(value, other);
}