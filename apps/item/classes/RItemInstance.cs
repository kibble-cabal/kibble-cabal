using Godot;
using KibbleCabal.Apps.Item;

[Tool]
[GlobalClass]
public sealed partial class RItemInstance : ExtensibleResource, ISpawnable
{
    private static class Keys
    {
        public const string AbilitySystemState = "AbilitySystemState";
    }
    
    [Export] public StringName ItemID = "";

    [Export] public int CreationTime;

    [Export]
    public AbilitySystemState? AbilitySystemState
    {
        get => GetSubResource<AbilitySystemState>(Keys.AbilitySystemState);
        set => SetSubResource(Keys.AbilitySystemState, value);
    }

    public RItem? GetItem() => ItemDB.Instance.Find(ItemID);

    public ItemInstanceScene Instantiate() => ItemInstanceScene.Instantiate(this);

    public Spawner GetSpawner() => new RItemInstanceSpawner(this);

    static RItemInstance()
    {
        #if TOOLS
        JSONSchema.GeneratorDB.Register(new JSONSchema.Generator
        {
            ClassName = nameof(RItemInstance),
            Path = "res://docs/schemas/ItemInstance.schema.json",
            Title = "Item Instance"
        });
        #endif
    }
}