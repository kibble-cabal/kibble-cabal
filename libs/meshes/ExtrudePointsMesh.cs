using Godot;

[Tool]
[GlobalClass]
public partial class ExtrudePointsMesh : ExtrudeVolumePackedVector2ArrayMesh
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