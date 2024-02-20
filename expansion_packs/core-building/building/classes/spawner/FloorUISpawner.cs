
using Godot;

// TODO
[GlobalClass]
public partial class FloorUISpawner : Spawner<RBuilding, Node>
{
    [Export]
    public int Index = -1;

    public FloorUISpawner() { }
    public FloorUISpawner(RBuilding building, int index)
    {
        SetResource(building);
        this.Index = index;
    }

    protected override Node? _Spawn(RBuilding resource, Node3D world)
    {
        var node = new Node();
        world.AddChild(node);
        return node;
    }

    protected override void _Update(RBuilding resource, Node node) { }
}