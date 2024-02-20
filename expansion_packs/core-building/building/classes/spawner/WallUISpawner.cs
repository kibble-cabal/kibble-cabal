using Godot;
using KibbleCabal.Core.Building.UI;

[GlobalClass]
public partial class WallUISpawner : Spawner<RBuilding, WallHUD>
{
    [Export]
    public int Index = -1;

    public WallUISpawner() { }
    public WallUISpawner(RBuilding building, int index)
    {
        SetResource(building);
        this.Index = index;
    }

    protected override WallHUD? _Spawn(RBuilding resource, Node3D world)
    {
        var node = WallHUD.Instantiate(resource, Index);
        world.AddChild(node);
        return node;
    }

    protected override void _Update(RBuilding resource, WallHUD node) => node.Update();
}