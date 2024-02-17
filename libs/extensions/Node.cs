using Godot;

public static class NodeExtensions
{
    public static void Set<[MustBeVariant] T>(this Node node, ref T prop, T value)
    {
        prop = value;
        node.NotifyPropertyListChanged();
    }

    public static bool CanQueueFree(this Node? node) => node != null
        && node.IsInsideTree()
        && !node.IsQueuedForDeletion();
}