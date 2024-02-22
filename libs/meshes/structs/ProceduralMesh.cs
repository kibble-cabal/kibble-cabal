using System;
using System.Linq;
using System.Threading.Tasks;
using Godot;

#nullable enable


public struct ProceduralMesh
{
    /* Private variables */
    private readonly SurfaceTool Surface = new();
    private Material[] StoredMaterials = [];
    private Triangle[] BakedTriangles = [];

    /* Public variables */
    public Func<IMeshComponent[]> GetComponents;
    public bool SmoothNormals = true;
    public Transform3D CustomTransform = Transform3D.Identity;
    public Material[] OverrideMaterials = [];

    /* Private methods */

    private void BakeTriangles()
    {
        BakedTriangles = GetComponents()
            .SelectMany(component => component.GetTriangles().SelectMany(tri => tri.GetTriangles()))
            .ToArray();
    }

    private void StoreMaterials(ArrayMesh mesh)
    {
        StoredMaterials = new Material[mesh.GetSurfaceCount()];
        for (int i = 0; i < StoredMaterials.Length; i++)
            StoredMaterials[i] = mesh.SurfaceGetMaterial(i);
    }

    private readonly void SetMaterials(ArrayMesh mesh)
    {
        int surfaceCount = mesh.GetSurfaceCount();
        for (int i = 0; i < Math.Min(StoredMaterials.Length, surfaceCount); i++)
            mesh.SurfaceSetMaterial(i, StoredMaterials[i]);
        for (int i = 0; i < Math.Min(OverrideMaterials.Length, surfaceCount); i++)
            mesh.SurfaceSetMaterial(i, OverrideMaterials[i]);
    }

    private void Clear()
    {
        BakedTriangles = [];
        // Surface.Clear();
    }

    private readonly (Vector3[] Vertices, Vector3[] Normals, Vector2[] UVs) BakeTriangle(int triangleIndex)
    {
        var offsets = (vertex: 0, normal: 0, uv: 0);
        Vector3[] bakedVertices = new Vector3[3];
        Vector3[] bakedNormals = new Vector3[3];
        Vector2[] bakedUVs = new Vector2[3];

        BakedTriangles[triangleIndex].BakeVertices(ref bakedVertices, ref offsets.vertex);
        BakedTriangles[triangleIndex].BakeNormals(ref bakedNormals, ref offsets.normal);
        BakedTriangles[triangleIndex].BakeUVs(ref bakedUVs, ref offsets.uv);

        return (bakedVertices, bakedNormals, bakedUVs);
    }

    private readonly bool IsBakeValid() => BakedTriangles.Length >= 0;

    private void GenerateSurface(ArrayMesh mesh, int surfaceIndex)
    {
        Surface.Begin(Mesh.PrimitiveType.Triangles);
        var triangleIndices = BakedTriangles
            .Select((triangle, index) => (triangle, index))
            .Where(val => val.triangle.Surface == surfaceIndex)
            .Select(val => val.index)
            .ToArray();

        foreach (var triangleIndex in triangleIndices)
        {
            var (vertices, normals, uvs) = BakeTriangle(triangleIndex);
            for (int i = 0; i < 3; i++)
            {
                Surface.SetUV(uvs[i]);
                if (!SmoothNormals)
                    Surface.SetNormal((normals[i] * CustomTransform.AffineInverse()).Normalized());
                Surface.AddVertex(vertices[i] * CustomTransform.AffineInverse());
            }
        }
        if (SmoothNormals) Surface.GenerateNormals();
        Surface.GenerateTangents();
        Surface.Commit(mesh);
        Surface.Clear();
    }

    /* Public methods */

    public ProceduralMesh(Func<IMeshComponent[]> getComponents, ArrayMesh mesh)
    {
        this.GetComponents = getComponents;
        this.Generate(mesh);
    }

    public void Generate(ArrayMesh mesh)
    {
        mesh.ClearSurfaces();
        StoreMaterials(mesh);
        Clear();
        BakeTriangles();
        if (!IsBakeValid()) return;
        foreach (var surfaceIndex in BakedTriangles.Select(triangle => triangle.Surface).Distinct().Order())
            GenerateSurface(mesh, surfaceIndex);
        SetMaterials(mesh);
    }

}