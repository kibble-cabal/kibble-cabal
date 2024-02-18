using BB;
using Godot;

public static class ObjectExtensions
{
    public static void Print(this object obj, params object[] messages) => GD.Print($"[{obj}] ".Gray(), string.Join("", messages).White());
    public static void PrintS(this object obj, params object[] messages) => GD.Print($"[{obj}] ".Gray(), string.Join(" ", messages).White());
    public static T? TryAs<[MustBeVariant] T>(this Variant variant) where T : class
    {
        try
        {
            return variant.As<T>();
        }
        catch
        {
            return null;
        }
    }
}