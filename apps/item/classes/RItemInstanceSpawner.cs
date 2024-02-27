using Godot;

[GlobalClass]
public sealed partial class RItemInstanceSpawner : Spawner<RItemInstance, Node3D>
{
    public RItemInstanceSpawner() { }
    public RItemInstanceSpawner(RItemInstance resource) : base(resource) { }

    private Vector3 _position;

    [Export]
    public Vector3 Position
    {
        get => _position;
        set => this.Set(ref _position, value);
    }

    protected override Node3D? _Spawn(RItemInstance item, Node3D world)
    {
        var node = item.Instantiate();
        node.Position = Position;
        world.AddChild(node);
        return node;
    }
    
    static RItemInstanceSpawner()
    {
        #if TOOLS
        JSONSchema.GeneratorDB.Register(new JSONSchema.Generator
        {
            ClassName = nameof(RItemInstanceSpawner),
            Path = "res://docs/schemas/ItemInstanceSpawner.schema.json",
            Title = "Item Instance Spawner"
        });
        #endif
    }
}