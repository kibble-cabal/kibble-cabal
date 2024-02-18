using Godot;

public static class NodeExtensions
{
    public static class GroupName
    {
        public static readonly StringName UIRoot = "UIRoot";
        public static readonly StringName LocationRoot = "LocationRoot";
    }

    public static void Set<[MustBeVariant] T>(this Node node, ref T prop, T value)
    {
        prop = value;
        node.NotifyPropertyListChanged();
    }

    public static bool CanQueueFree(this Node? node) => node != null
        && node.IsInsideTree()
        && !node.IsQueuedForDeletion();

    public static Node? GetUIRoot(this Node node) => node.GetTree().GetFirstNodeInGroup(GroupName.UIRoot);
    public static Node3D? GetLocationRoot(this Node node) => node.GetTree().GetFirstNodeInGroup(GroupName.LocationRoot) as Node3D;
}