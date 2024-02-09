using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class CompoundMesh : ArrayMesh
{
    private Array<Mesh> Meshes = [];
    private Callable GenerateCallable;

    [Export]
    public Array<Mesh> meshes
    {
        get => Meshes;
        set
        {
            foreach (Mesh mesh in Meshes)
                mesh?.TryDisconnectChanged(GenerateCallable);
            foreach (Mesh mesh in value)
                mesh?.TryConnectChanged(GenerateCallable);
            Meshes = value;
            generate();
        }
    }

    public CompoundMesh() => this.GenerateCallable = new Callable(this, "generate");

    private (int meshIndex, int surfaceIndex) GetIndices(int index)
    {
        int currentIndex = 0, meshIndex = 0;
        foreach (Mesh mesh in Meshes)
        {
            int surfaceCount = mesh.GetSurfaceCount();
            if (index >= currentIndex && index < currentIndex + surfaceCount)
                return (meshIndex, index - currentIndex);
            currentIndex += surfaceCount;
            meshIndex += 1;
        }
        return (-1, -1);
    }

    private void ForSurface(int index, System.Action<Mesh, int> fn)
    {
        var (meshIndex, surfaceIndex) = GetIndices(index);
        if (meshIndex >= 0 && surfaceIndex >= 0) fn(Meshes[meshIndex], surfaceIndex);
    }

    private T MapSurface<T>(int index, System.Func<Mesh, int, T> fn)
    {
        var (meshIndex, surfaceIndex) = GetIndices(index);
        if (meshIndex >= 0 && surfaceIndex >= 0) return fn(Meshes[meshIndex], surfaceIndex);
        return default;
    }

    public override Aabb _GetAabb()
    {
        Aabb aabb = new();
        foreach (Mesh mesh in Meshes) aabb = aabb.Merge(mesh.GetAabb());
        return aabb;
    }

    public override int _GetSurfaceCount()
    {
        int count = 0;
        foreach (Mesh mesh in Meshes) count += mesh.GetSurfaceCount();
        return count;
    }

    public override Array _SurfaceGetArrays(int index) => MapSurface(index, (mesh, surfaceIndex) => mesh.SurfaceGetArrays(surfaceIndex));

    public override void _SurfaceSetMaterial(int index, Material material) => ForSurface(index, (mesh, surfaceIndex) => mesh.SurfaceSetMaterial(surfaceIndex, material));

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
        EmitChanged();
    }
}