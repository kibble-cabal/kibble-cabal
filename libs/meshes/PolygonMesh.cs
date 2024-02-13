using Godot;

[Tool]
[GlobalClass]
public partial class PolygonPointsMesh : PolygonMeshBase
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