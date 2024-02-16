
using Godot;

[GlobalClass]
public partial class RItemPhysics : ExtensibleResource
{
    private PackedScene? _scene;

    [Export]
    public PackedScene? Scene
    {
        get => _scene;
        set => this.Set(ref _scene, value);
    }
}