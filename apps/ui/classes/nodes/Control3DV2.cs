using Godot;

[GlobalClass]
public partial class Control3DV2 : Control
{
    [Export]
    public Vector3 LocalPosition;

    [Export]
    public bool Center = true;

    private Node? Parent;
    private Viewport? Viewport;
    private Camera3D? Camera;

    public override void _Ready()
    {
        Parent = GetParent();
        Viewport = GetViewport();
        Camera = Viewport?.GetCamera3D();
    }

    public override void _Process(double delta)
    {
        if (!IsInsideTree() || !Visible || Parent is null || Camera is null) return;
        var totalPositon = LocalPosition;
        if (Parent is Node3D parent) totalPositon += parent.GlobalPosition;
        Position = Camera.UnprojectPosition(totalPositon);
        if (Center) Position -= Size / 2;
    }
}