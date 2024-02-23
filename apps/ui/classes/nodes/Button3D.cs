using Godot;

[GlobalClass]
public partial class Button3D : Button, IControl3D
{
    [Export]
    public Vector3 LocalPosition { get; set; }

    [Export]
    public bool Center { get; set; } = true;

    [Export]
    public Vector2 ScreenOffset { get; set; }

    private Node? Parent;
    private Camera3D? Camera;

    public override void _Ready()
    {
        Parent = GetParent();
        Camera = GetViewport()?.GetCamera3D();
    }

    public override void _Process(double delta) => IControl3D.ProcessPosition(this, Parent, Camera);
}