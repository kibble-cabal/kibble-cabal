
using Godot;

[Tool]
[GlobalClass]
public partial class RItemPhysics : ExtensibleResource
{
    private static class Keys
    {
        public const string Scene = "Scene";
    }
    
    [Export]
    public PackedScene? Scene
    {
        get => GetSubResource<PackedScene>(Keys.Scene);
        set => SetSubResource(Keys.Scene, value);
    }
    
    static RItemPhysics()
    {
        #if TOOLS
        JSONSchema.GeneratorDB.Register(new JSONSchema.Generator
        {
            ClassName = nameof(RItemPhysics),
            Path = "res://docs/schemas/ItemPhysics.schema.json",
            Title = "Item Physics Data"
        });
        #endif
    }
}