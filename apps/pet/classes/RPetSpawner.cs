using Godot;

[GlobalClass]
public sealed partial class RPetSpawner : Spawner<RPet, Node3D>
{
    public static readonly PackedScene Scene = GD.Load<PackedScene>("res://apps/pet/scenes/pet_scene.tscn");

    private Vector3 _position;

    public Vector3 Position
    {
        get => _position;
        set => this.Set(ref _position, value);
    }

    protected override Node3D? _Spawn(RPet resource, Node3D world)
    {
        var node = Scene.Instantiate<Node3D>();
        node.Set("resource", resource);
        world.AddChild(node);
        return node;
    }
}