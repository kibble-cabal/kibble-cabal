using Godot;

[GlobalClass]
public partial class BuildingSpawner : Spawner<RBuilding, MeshInstance3D>
{
    public BuildingSpawner() { }
    public BuildingSpawner(RBuilding building) => SetResource(building);

    protected override MeshInstance3D? _Spawn(RBuilding resource, Node3D world)
    {
        var node = new MeshInstance3D() { Mesh = resource.GenerateMesh() };
        world.AddChild(node);
        return node;
    }

    protected override void _Update(RBuilding resource, MeshInstance3D node) => node.Mesh = resource.GenerateMesh();
}