using Godot;

[Tool]
[GlobalClass]
public partial class PolylinePointsMesh : PolylineMeshBase
{
    /* Public variables */
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