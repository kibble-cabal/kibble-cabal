using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class CompoundMesh : ArrayMesh
{
    private Array<Mesh> Meshes = [];

    [Export]
    public Array<Mesh> meshes
    {
        get => Meshes;
        set
        {
            foreach (Mesh mesh in Meshes)
                mesh?.TryDisconnectChanged(new Callable(this, "generate"));
            foreach (Mesh mesh in value)
                mesh?.TryConnectChanged(new Callable(this, "generate"));
            Meshes = value;
            generate();
        }
    }

    public override Aabb _GetAabb()
    {
        Aabb aabb = new();
        foreach (Mesh mesh in Meshes) aabb = aabb.Merge(mesh.GetAabb());
        return aabb;
    }

    public void generate()
    {
        ClearSurfaces();
        foreach (Mesh mesh in Meshes)
        {
            if (mesh == null) continue;
            for (int surfaceIndex = 0; surfaceIndex < mesh.GetSurfaceCount(); surfaceIndex++)
            {
                AddSurfaceFromArrays(
                    PrimitiveType.Triangles,
                    mesh.SurfaceGetArrays(surfaceIndex)
                );
                SurfaceSetMaterial(
                    GetSurfaceCount() - 1,
                    mesh.SurfaceGetMaterial(surfaceIndex)
                );
            }
        }
    }
}