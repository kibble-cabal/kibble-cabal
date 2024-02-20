using Godot;
using KibbleCabal.Apps.Pet;

[GlobalClass]
public sealed partial class RPetSpawner : Spawner<RPet, Node3D>
{
    public static readonly PackedScene Scene = GD.Load<PackedScene>("res://apps/pet/scenes/pet_scene.tscn");

    protected override Node3D? _Spawn(RPet resource, Node3D world)
    {
        var node = Scene.Instantiate<PetScene>();
        node.Resource = resource;
        world.AddChild(node);
        return node;
    }
}