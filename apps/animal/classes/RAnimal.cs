using Godot;

[GlobalClass]
public partial class RAnimal : ExtensibleResource, IIdentifiable<StringName>
{
    public const string SchemaPath = "res://apps/animal/animal_resource.schema.json";

    private StringName _name = "";
    private PackedScene? _spriteScene;
    private float _collisionRadius = 0.15f;
    private float _detectionRadius = 1000f;
    private float _speed = 0.5f;

    public StringName ID { get => _name; }

    [Export]
    public StringName Name
    {
        get => _name;
        set => this.Set(ref _name, value);
    }

    [Export]
    public PackedScene? SpriteScene
    {
        get => _spriteScene;
        set => this.Set(ref _spriteScene, value);
    }

    [Export(PropertyHint.Range, "0,1")]
    public float Speed
    {
        get => _speed;
        set => this.Set(ref _speed, value);
    }

    /// <summary>
    /// The radius (in meters) of this animal's body.
    /// </summary>
    [Export]
    public float CollisionRadius
    {
        get => _collisionRadius;
        set => this.Set(ref _collisionRadius, value);
    }

    /// <summary>
    /// The radius (in meters) that this animal can detect items.
    /// </summary>
    [Export]
    public float DetectionRadius
    {
        get => _detectionRadius;
        set => this.Set(ref _detectionRadius, value);
    }

    static RAnimal()
    {
        #if TOOLS
        JSON.Schema.GeneratorDB.Register(new JSON.Schema.Generator
        {
            ClassName = nameof(RAnimal),
            Path = "res://docs/schemas/Animal.schema.json",
            Title = "Animal"
        });
        #endif
    }
}