
using Godot;
using KibbleCabal.Core.Building.UI;

[GlobalClass]
public partial class BuildingUISpawner : Spawner<RBuilding, BuildingHUD>
{
    public BuildingUISpawner() { }
    public BuildingUISpawner(RBuilding building) => SetResource(building);

    protected override BuildingHUD? _Spawn(RBuilding resource, Node3D world)
    {
        var node = BuildingHUD.Instantiate(resource);
        world.AddChild(node);
        return node;
    }

    protected override void _Update(RBuilding resource, BuildingHUD node) => node.Update();
}