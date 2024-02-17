using Godot;

[GlobalClass]
public sealed partial class RItemInstance : ExtensibleResource, ISpawnable
{
    public static readonly PackedScene Scene = (PackedScene)GD.Load("res://apps/item/scenes/item_instance_scene.tscn");

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

    public Node3D Instantiate()
    {
        var scene = (Node3D)Scene.Instantiate();
        scene.Set("item", this);
        return scene;
    }

    public Spawner GetSpawner() => new RItemInstanceSpawner(this);
}