using Godot;

[GlobalClass]
public sealed partial class RSubTree : ExtensibleResource
{
    private static class Keys
    {
        public static readonly string SubTree = "SubTree";
    }

    private StringName _hook = "";
    private int _priority = 1;

    [Export]
    public StringName Hook
    {
        get => _hook;
        set => this.Set(ref _hook, value);
    }

    [Export]
    public BehaviorTree? SubTree
    {
        get => GetSubResource<BehaviorTree>(Keys.SubTree);
        set => SetSubResource(Keys.SubTree, value);
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

    static RSubTree()
    {
        #if TOOLS
        JSONSchema.GeneratorDB.Register(new JSONSchema.Generator
        {
            ClassName = nameof(RSubTree),
            Path = "res://docs/schemas/SubTree.schema.json",
            Title = "SubTree"
        });
        #endif
    }
}