using System.Linq;
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
    public static string ToPrecisionString(this float value, int precision = 2) => precision <= 0 ? Mathf.RoundToInt(value).ToString() : $"{Mathf.Snapped(value, 1.0f / Mathf.Pow(10, precision))}".PadDecimals(2);
    public static string ToPrecisionString(this double value, int precision = 2) => precision <= 0 ? Mathf.RoundToInt(value).ToString() : $"{System.Math.Round(value, precision)}".PadDecimals(2);
    public static bool IsEqualApprox(this float value, float other) => Mathf.IsEqualApprox(value, other);
    public static float ToRad(this float degrees) => Mathf.DegToRad(degrees);
    public static float ToDeg(this float rad) => Mathf.RadToDeg(rad);
    public static float Cos(this float value) => Mathf.Cos(value);
    public static float Sin(this float value) => Mathf.Sin(value);
    public static float Clamp(this float value, float min, float max) => Mathf.Clamp(value, min, max);
    public static float Wrap(this float value, float min, float max)
    {
        var v = value;
        var range = max - min;
        while (v < min) v += range;
        while (v > max) v -= range;
        return v;
    }
    public static float Remap(this float from, float fromMin, float fromMax, float toMin, float toMax)
    {
        var fromAbs = from - fromMin;
        var fromMaxAbs = fromMax - fromMin;

        var normal = fromAbs / fromMaxAbs;

        var toMaxAbs = toMax - toMin;
        var toAbs = toMaxAbs * normal;

        var to = toAbs + toMin;

        return to;
    }
    public static float Map(this float value, float min, float max) => value.Remap(0, 1, min, max);
}