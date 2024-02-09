using System;
using System.Linq;
using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class CurveMesh : ArrayMesh
{
    /* Private variables */
    protected Vector2[] BakedPoints = [];
    protected Array<Vector3> BakedVertices = [];
    protected Array<Vector2> BakedUVs = [];
    protected SurfaceTool Surface = new();
    protected bool Flip = false;
    protected Curve2D Curve;
    protected int TessellationStages = 3;
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
    private Curve2D curve
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

    public CurveMesh()
    {
        GenerateCallable = new Callable(this, "generate");
    }

    /* Private methods */

    protected virtual void Clear()
    {
        BakedPoints = [];
        BakedVertices.Clear();
        BakedUVs.Clear();
        Surface.Clear();
        ClearSurfaces();
    }

    protected virtual bool BakePoints()
    {
        if (!CanBake()) return false;
        BakedPoints = Curve.Tessellate(TessellationStages);
        return BakedPoints.Length >= 2;
    }

    protected virtual bool Bake()
    {
        if (!BakePoints()) return false;
        var bakedPoints = Geometry2D.TriangulatePolygon(BakedPoints);
        if (Flip) bakedPoints = bakedPoints.Reverse().ToArray();
        foreach (int vertexIndex in bakedPoints)
        {
            var vertex = BakedPoints[vertexIndex];
            BakedVertices.Add(new Vector3(vertex.X, 0, vertex.Y));
            BakedUVs.Add(vertex);
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
        && BakedVertices.Count >= 3
        && BakedVertices.Count % 3 == 0
        && BakedUVs.Count == BakedVertices.Count
    );

    /* Public Methods */

    public virtual void generate()
    {
        Clear();
        if (!Bake()) return;
        Surface.Begin(PrimitiveType.Triangles);
        for (int i = 0; i < BakedVertices.Count; i++)
        {
            if (BakedUVs.Count > i)
                Surface.SetUV(BakedUVs[i]);
            Surface.AddVertex(BakedVertices[i]);
        }
        Surface.GenerateNormals();
        Surface.GenerateTangents();
        Surface.Commit(this);
        EmitChanged();
    }
}