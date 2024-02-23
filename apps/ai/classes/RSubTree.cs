using Godot;

[GlobalClass]
public sealed partial class RSubTree : ExtensibleResource, IIdentifiable<StringName>
{
    private static class Keys
    {
        public static readonly string SubTree = "SubTree";
    }

    private StringName _id = "";
    private int _priority = 1;

    [Export]
    public StringName ID
    {
        get => _id;
        set => this.Set(ref _id, value);
    }

    [Export]
    public BehaviorTree? SubTree
    {
        get => GetSubresource<BehaviorTree>(Keys.SubTree);
        set => SetSubresource(Keys.SubTree, value);
    }

    /// <summary>
    /// Defines the order in which subtrees will be added to the behavior tree.
    /// Will be run in order of highest priority to lowest priority.
    /// </summary>
    [Export]
    public int Priority
    {
        get => _priority;
        set => this.Set(ref _priority, value);
    }
}