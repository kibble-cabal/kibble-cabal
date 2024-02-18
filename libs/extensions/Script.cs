using Godot;

public static class ScriptExtensions
{
    public static GodotObject? New(this Script script)
    {
        if (script is GDScript gdScript) return gdScript.New().TryAs<GodotObject>();
        else if (script is CSharpScript csScript) return csScript.New().TryAs<GodotObject>();
        return null;
    }
}