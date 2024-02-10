using System;
using System.Linq;
using Godot;

#nullable enable


public struct ProceduralMesh
{
    /* Private variables */
    internal SurfaceTool Surface = new();
    internal Material[] StoredMaterials = [];
    internal Triangle[] BakedTriangles = [];
    internal Vector3[] BakedVertices = [];
    internal Vector3[] BakedNormals = [];
    internal Vector2[] BakedUVs = [];

    /* Public variables */

    public Func<Triangle[]> GetTriangles;
    public Func<int[]>? GetSurfaces;
    public bool FlipFaces = false;
    public bool SmoothNormals = true;
    public Transform3D CustomTransform = Transform3D.Identity;
    public Material[] OverrideMaterials = [];

    /* Private methods */

    internal void BakeTriangles()
    {
        BakedTriangles = GetTriangles().Where(triangle => triangle.IsValid()).ToArray();
        if (FlipFaces) BakedTriangles = BakedTriangles.Select(triangle => triangle.Inverted()).ToArray();
    }

    internal void BakeVertices()
    {
        int vertexOffset = 0;
        BakedVertices = new Vector3[BakedTriangles.Length * 3];
        foreach (var triangle in BakedTriangles)
            triangle.BakeVertices(ref BakedVertices, ref vertexOffset);
    }

    internal void BakeNormals()
    {
        if (SmoothNormals) return;
        int normalOffset = 0;
        BakedNormals = new Vector3[BakedTriangles.Length * 3];
        foreach (var triangle in BakedTriangles)
            triangle.BakeNormals(ref BakedNormals, ref normalOffset);
    }

    internal void BakeUVs()
    {
        BakedUVs = new Vector2[BakedTriangles.Length * 3];
        int uvOffset = 0;
        foreach (var triangle in BakedTriangles)
            triangle.BakeUVs(ref BakedUVs, ref uvOffset);
    }

    internal void StoreMaterials(ArrayMesh mesh)
    {
        StoredMaterials = new Material[mesh.GetSurfaceCount()];
        for (int i = 0; i < StoredMaterials.Length; i++)
            StoredMaterials[i] = mesh.SurfaceGetMaterial(i);
    }

    internal void SetMaterials(ArrayMesh mesh)
    {
        int surfaceCount = mesh.GetSurfaceCount();
        for (int i = 0; i < Math.Min(StoredMaterials.Length, surfaceCount); i++)
            mesh.SurfaceSetMaterial(i, StoredMaterials[i]);
        for (int i = 0; i < Math.Min(OverrideMaterials.Length, surfaceCount); i++)
            mesh.SurfaceSetMaterial(i, OverrideMaterials[i]);
    }

    internal void Bake()
    {
        BakeTriangles();
        BakeVertices();
        BakeNormals();
        BakeUVs();
    }

    internal void Clear()
    {
        BakedTriangles = [];
        BakedVertices = [];
        BakedNormals = [];
        BakedUVs = [];
        Surface.Clear();
    }

    internal readonly bool IsBakeValid() => (
        BakedTriangles.Length >= 0
        && BakedVertices.Length >= 3
        && BakedVertices.Length % 3 == 0
        && BakedNormals.Length % 3 == 0
        && BakedUVs.Length % 3 == 0
    );

    internal void GenerateSurface(ArrayMesh mesh, int startTriangleIndex, int endTriangleIndex)
    {
        Surface.Begin(Mesh.PrimitiveType.Triangles);
        for (int i = startTriangleIndex * 3; i < endTriangleIndex * 3; i++)
        {
            if (BakedUVs.Length > i)
                Surface.SetUV(BakedUVs[i]);
            if (!SmoothNormals && BakedNormals.Length > i)
                Surface.SetNormal((BakedNormals[i] * CustomTransform.AffineInverse()).Normalized());
            Surface.AddVertex(BakedVertices[i] * CustomTransform.AffineInverse());
        }
        if (SmoothNormals) Surface.GenerateNormals();
        Surface.GenerateTangents();
        Surface.Commit(mesh);
    }

    internal int[] GetSurfaceIndices()
    {
        if (GetSurfaces is Func<int[]> getSurfaces)
            return [0, .. getSurfaces(), BakedTriangles.Length];
        return [0, BakedTriangles.Length];
    }

    /* Public methods */

    public ProceduralMesh(Func<Triangle[]> getTriangles, Func<int[]>? getSurfaces, ArrayMesh mesh)
    {
        this.GetTriangles = getTriangles;
        this.GetSurfaces = getSurfaces;
        this.Generate(mesh);
    }

    public bool Generate(ArrayMesh mesh)
    {
        mesh.ClearSurfaces();
        StoreMaterials(mesh);
        Clear();
        Bake();
        if (!IsBakeValid()) return false;
        int[] surfaces = GetSurfaceIndices();
        for (int i = 0; i < surfaces.Length - 1; i++)
            GenerateSurface(mesh, surfaces[i], surfaces[i + 1]);
        SetMaterials(mesh);
        return true;
    }
}