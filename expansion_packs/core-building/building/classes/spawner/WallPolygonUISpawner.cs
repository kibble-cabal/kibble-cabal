using Godot;

[GlobalClass]
public partial class WallPolygonUISpawner : Spawner<RBuilding, PolygonEditor3D>
{
    [Export]
    public int Index = -1;

    public WallPolygonUISpawner() { }
    public WallPolygonUISpawner(RBuilding building, int index)
    {
        this.Resource = building;
        this.Index = index;
    }

    protected override PolygonEditor3D? _Spawn(RBuilding resource, Node3D world)
    {
        var node = new PolygonEditor3D()
        {
            History = BuildModeState.GetHistory(),
            EnableHandles = true,
            ActiveModulate = Colors.Cyan,
            HandleActiveModulate = Colors.Cyan,
            CustomSnapMethod = position =>
            {
                var snapped = resource.Snap(position, 0.2f);
                // If the position was snapped to itself, return the unsnapped position
                if (resource.Snap<Wall>(Index, position).IsEqualApprox(snapped))
                    return position;
                return snapped;
            },
        };
        node.PointChanged += OnPointChanged;
        node.InHandleChanged += OnHandleChanged;
        node.OutHandleChanged += OnHandleChanged;
        world.AddChild(node);
        return node;
    }

    protected override void _Update(RBuilding resource, PolygonEditor3D node)
    {
        if (resource.Get<Wall>(Index) is Wall wall)
        {
            node.Polygon = [wall.Start, wall.End];
            node.InHandlePositions = [wall.StartHandle, wall.EndHandle];
            node.OutHandlePositions = [wall.StartHandle, wall.EndHandle];
            node.Size = wall.Thickness * 2;
            node.UpdatePoints();
        }
    }

    private void OnPointChanged(int pointIndex, Vector2 position)
    {
        if (Resource?.Get<Wall>(Index) is Wall wall)
        {
            if (pointIndex == 0) wall.Start = position;
            else wall.End = position;
        }
    }

    private void OnHandleChanged(int handleIndex, Vector2 position)
    {
        if (Resource?.Get<Wall>(Index) is Wall wall)
        {
            if (handleIndex == 0) wall.StartHandle = position;
            else if (handleIndex == 1) wall.EndHandle = position;
        }
    }
}