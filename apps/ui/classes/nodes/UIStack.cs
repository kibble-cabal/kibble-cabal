
using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class UIStack : Control
{
    public enum PushMode
    {
        Replace,
        Append
    }

    [Export]
    public PushMode Mode = PushMode.Replace;

    public Array<Control> Stack { get; private set; } = [];

    public Control? Current => Stack.LastOrDefault();

    public void Push(Control scene)
    {
        // Remove previous scene
        if (Stack.Count > 0 && Mode == PushMode.Replace)
            RemoveChild(Current);
        // Add next scene
        Stack.Add(scene);
        AddChild(scene);
    }

    public void Pop()
    {
        if (Current is not null)
        {
            Current.QueueFree();
            Stack.Pop();
        }
        if (Current is not null && !Current.IsInsideTree())
            AddChild(Current);
    }

    public void Clear()
    {
        this.QueueFreeChildren();
        Stack.Clear();
    }
}