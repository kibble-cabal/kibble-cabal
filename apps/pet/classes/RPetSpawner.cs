using Godot;
using KibbleCabal.Apps.Pet;

[GlobalClass]
public sealed partial class RPetSpawner : Spawner<RPet, PetScene>
{
    public static readonly PackedScene Scene = GD.Load<PackedScene>("res://apps/pet/scenes/pet_scene.tscn");

    public RPetSpawner() { }
    public RPetSpawner(RPet resource) => SetResource(resource);

    protected override PetScene? _Spawn(RPet resource, Node3D world)
    {
        var node = Scene.Instantiate<PetScene>();
        node.Resource = resource;
        world.AddChild(node);
        return node;
    }
    
    static RPetSpawner()
    {
        #if TOOLS
        JSONSchema.GeneratorDB.Register(new JSONSchema.Generator
        {
            ClassName = nameof(RPetSpawner),
            Path = "res://docs/schemas/PetSpawner.schema.json",
            Title = "Pet Spawner"
        });
        #endif
    }
}