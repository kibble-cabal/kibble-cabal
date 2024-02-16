using Godot;

public static class ResourceExtensions
{
    public static void Set<[MustBeVariant] T>(this Resource resource, ref T prop, T value)
    {
        prop = value;
        resource.EmitChanged();
    }
}