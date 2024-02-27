using Godot;

[Tool]
[GlobalClass]
public partial class RItem : ExtensibleResource, IIdentifiable<StringName>
{
    private static class Keys
    {
        public const string Physics = "Physics";
        public const string Retail = "Retail";
        public const string AbilitySystemState = "AbilitySystemState";
    }

    [Export] public StringName ID { get; set; } = "";

    [Export] public StringName DisplayName = "";

    [Export(PropertyHint.MultilineText)] public string Description = "";

    [Export] public Texture2D? Icon;

    [Export]
    public AbilitySystemState AbilitySystemState
    {
        get => ExpectSubResource<AbilitySystemState>(Keys.AbilitySystemState);
        set => SetSubResource(Keys.AbilitySystemState, value);
    }

    [Export]
    public RItemPhysics? Physics
    {
        get => GetSubResource<RItemPhysics>(Keys.Physics);
        set => SetSubResource(Keys.Physics, value);
    }

    [Export]
    public RItemRetail? Retail
    {
        get => GetSubResource<RItemRetail>(Keys.Retail);
        set => SetSubResource(Keys.Retail, value);
    }

    public RItemInstance Instantiate() => new()
    {
        ItemID = ID,
        CreationTime = DateTimeSubSystem.Time,
        AbilitySystemState = AbilitySystemState.Duplicate(false) as AbilitySystemState
    };

    static RItem()
    {
        #if TOOLS
        JSONSchema.GeneratorDB.Register(new JSONSchema.Generator
        {
            ClassName = nameof(RItem),
            Path = "res://docs/schemas/Item.schema.json",
            Title = "Item"
        });
        #endif
    }
}