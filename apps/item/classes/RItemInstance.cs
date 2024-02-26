using Godot;
using KibbleCabal.Apps.Item;

[GlobalClass]
public sealed partial class RItemInstance : ExtensibleResource, ISpawnable
{
    private StringName _itemID = "";
    private int _creationTime;

    [Export]
    public StringName ItemID
    {
        get => _itemID;
        set => this.Set(ref _itemID, value);
    }

    [Export]
    public int CreationTime
    {
        get => _creationTime;
        set => this.Set(ref _creationTime, value);
    }

    public RItem? GetItem() => ItemDB.Instance.Find(_itemID);

    public ItemInstanceScene Instantiate() => ItemInstanceScene.Instantiate(this);

    public Spawner GetSpawner() => new RItemInstanceSpawner(this);

    static RItemInstance()
    {
        JSONSchema.GeneratorDB.Register(new JSONSchema.Generator
        {
            ClassName = nameof(RItemInstance),
            Path = "res://docs/schemas/ItemInstance.schema.json",
            Title = "Item Instance"
        });
    }
}