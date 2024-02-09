using System;
using System.Linq;
using Godot;

#nullable enable

[Tool]
[GlobalClass]
public partial class CurveMesh : ArrayMesh
{
    /* Private variables */
    protected Vector2[] BakedPoints = [];
    protected Vector3[] BakedVertices = [];
    protected Vector2[] BakedUVs = [];
    protected SurfaceTool Surface = new();
    protected bool Flip = false;
    protected Curve2D? Curve;
    protected int TessellationStages = 3;
    protected float TessellationToleranceDegrees = 4;
    protected Transform3D CustomTransform = Transform3D.Identity;
    protected BaseMaterial3D? Material;
    protected Callable GenerateCallable;

    /* Public variables */

    [Export]
    private bool flip
    {
        get => Flip;
        set
        {
            Flip = value;
            generate();
        }
    }

    [Export]
    private Curve2D? curve
    {
        get => Curve;
        set
        {
            Curve?.TryDisconnectChanged(GenerateCallable);
            value?.TryConnectChanged(GenerateCallable);
            Curve = value;
            generate();
        }
    }

    [Export]
    private int tessellation_stages
    {
        get => TessellationStages;
        set
        {
            TessellationStages = value;
            generate();
        }
    }

    [Export]
    private float tessellation_tolerance_degrees
    {
        get => TessellationToleranceDegrees;
        set
        {
            TessellationToleranceDegrees = value;
            generate();
        }
    }

    [Export]
    private Transform3D custom_transform
    {
        get => CustomTransform;
        set
        {
            CustomTransform = value;
            generate();
        }
    }

    [Export]
    private BaseMaterial3D? material
    {
        get => Material;
        set
        {
            Material = value;
            generate();
        }
    }

    public CurveMesh()
    {
        GenerateCallable = new Callable(this, "generate");
    }

    /* Private methods */

    protected void ResizeMeshArrays(int size)
    {
        BakedVertices = new Vector3[size];
        BakedUVs = new Vector2[size];
    }

    protected virtual void Clear()
    {
        BakedPoints = [];
        ResizeMeshArrays(0);
        Surface.Clear();
        ClearSurfaces();
    }

    protected virtual bool BakePoints()
    {
        if (!CanBake()) return false;
        BakedPoints = Curve?.Tessellate(TessellationStages, TessellationToleranceDegrees) ?? [];
        return BakedPoints.Length >= 2;
    }

    protected virtual bool Bake()
    {
        if (!BakePoints()) return false;
        var bakedPointIndices = Geometry2D.TriangulatePolygon(BakedPoints);
        ResizeMeshArrays(bakedPointIndices.Length);
        if (Flip) bakedPointIndices = bakedPointIndices.Reverse().ToArray();
        int i = 0;
        foreach (int vertexIndex in bakedPointIndices)
        {
            var vertex = BakedPoints[vertexIndex];
            BakedVertices[i] = new Vector3(vertex.X, 0, vertex.Y);
            BakedUVs[i] = vertex;
            i += 1;
        }
        return IsBakeValid();
    }

    protected virtual bool CanBake() => (
        Surface != null
        && Curve != null
        && Curve.PointCount >= 2
    );

    protected virtual bool IsBakeValid() => (
        BakedPoints.Length >= 2
        && BakedVertices.Length >= 3
        && BakedVertices.Length % 3 == 0
        && BakedUVs.Length == BakedVertices.Length
    );

    /* Public Methods */

    public virtual void generate()
    {
        Clear();
        if (!Bake()) return;
        Surface.Begin(PrimitiveType.Triangles);
        for (int i = 0; i < BakedVertices.Length; i++)
        {
            if (BakedUVs.Length > i)
                Surface.SetUV(BakedUVs[i]);
            Surface.AddVertex(BakedVertices[i] * CustomTransform.AffineInverse());
        }
        Surface.GenerateNormals();
        Surface.GenerateTangents();
        Surface.Commit(this);
        if (GetSurfaceCount() > 0) SurfaceSetMaterial(0, Material);
        EmitChanged();
    }
}