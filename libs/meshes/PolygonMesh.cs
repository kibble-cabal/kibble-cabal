using Godot;

[Tool]
[GlobalClass]
public partial class PolygonMesh : PolygonMeshBase
{
    [Export]
    public Vector2[] points
    {
        get => Points;
        set
        {
            Points = value;
            InternalMesh.Generate(this);
        }
    }
}