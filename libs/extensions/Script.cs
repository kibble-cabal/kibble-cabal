using Godot;

public static class ScriptExtensions
{
    public static T? New<[MustBeVariant] T>(this Script script) where T : class
    {
        if (script is GDScript gdScript) return gdScript.New().TryAs<T>();
        else if (script is CSharpScript csScript) return csScript.New().TryAs<T>();
        return null;
    }
    public static GodotObject? New(this Script script) => script.New<GodotObject>();
}