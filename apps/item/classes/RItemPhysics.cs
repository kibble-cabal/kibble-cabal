
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
    
    static RItemPhysics()
    {
        JSONSchema.GeneratorDB.Register(new JSONSchema.Generator
        {
            ClassName = nameof(RItemPhysics),
            Path = "res://docs/schemas/ItemPhysics.schema.json",
            Title = "Item Physics Data"
        });
    }
}