using Godot;

#nullable enable

using Tri = (Godot.Vector3 A, Godot.Vector3 B, Godot.Vector3 C);
using Segment = (Godot.Vector3 A, Godot.Vector3 B);

using Godot.Collections;
using System.Linq;
using System;

public static class ToPackedArrayExtension
{
    public static T[] ToPackedArray<[MustBeVariant] T>(this Array<T> array)
    {
        T[] newArray = new T[array.Count];
        array.CopyTo(newArray, 0);
        return newArray;
    }
}

struct Triangle
{
    public Tri Points;

    public Triangle(Vector3 a, Vector3 b, Vector3 c)
    {
        this.Points = (a, b, c);
    }

    public Vector3 GetNormal() => (Points.B - Points.C).Cross(Points.A - Points.C).Normalized();
    public Vector3 GetInvertedNormal() => (Points.B - Points.A).Cross(Points.C - Points.A).Normalized();

    public Triangle Inverted()
    {
        var (A, _, C) = Points;
        Points.A = C;
        Points.C = A;
        return this;
    }

    public void BakeVertices(ref Vector3[] vertices, ref int offset)
    {
        vertices[offset] = Points.A;
        vertices[offset + 1] = Points.B;
        vertices[offset + 2] = Points.C;
        offset += 3;
    }
    public void BakeNormals(ref Vector3[] normals, ref int offset)
    {
        var normal = GetNormal();
        for (int i = 0; i < 3; i++) normals[offset + i] = normal;
        offset += 3;
    }
    /// <summary>
    /// Bakes in world coordinates. Assumes the normal is pointed up, will be updated later.
    /// </summary>
    public void BakeUVs(ref Vector2[] uvs, ref int offset)
    {
        uvs[offset] = Points.A.FromVector3();
        uvs[offset + 1] = Points.B.FromVector3();
        uvs[offset + 2] = Points.C.FromVector3();
        offset += 3;
    }
}

struct ExtrudeSegment
{
    public Segment Points;
    public float Offset;
    public Vector3 Direction;
    public float Length;
    public float SegmentLength => Points.A.DistanceTo(Points.B);

    public ExtrudeSegment(Segment points, Vector3 direction, float length, float offset)
    {
        this.Points = points;
        this.Direction = direction;
        this.Length = length;
        this.Offset = offset;
    }

    public (Triangle A, Triangle B) GetTriangles()
    {
        var points = (
            Points.A,
            Points.B,
            C: Points.A + Direction * Length,
            D: Points.B + Direction * Length
        );
        return (
            new Triangle(points.C, points.B, points.A),
            new Triangle(points.B, points.C, points.D)
        );
    }

    public void BakeVertices(ref Vector3[] vertices, ref int offset)
    {
        var (A, B) = GetTriangles();
        A.BakeVertices(ref vertices, ref offset);
        B.BakeVertices(ref vertices, ref offset);
    }
    public void BakeNormals(ref Vector3[] normals, ref int offset)
    {
        var (A, B) = GetTriangles();
        A.BakeNormals(ref normals, ref offset);
        B.BakeNormals(ref normals, ref offset);
    }

    public void BakeUVs(ref Vector2[] uvs, ref int offset)
    {
        var o = new Vector2(Offset, 0);
        var e = new Vector2(SegmentLength, Direction.Length() * Length);
        var tl = o + e * new Vector2(0, 0);
        var tr = o + e * new Vector2(1, 0);
        var br = o + e * new Vector2(1, 1);
        var bl = o + e * new Vector2(0, 1);
        Vector2[] nextUvs = [tl, br, bl, br, tl, tr];
        nextUvs.CopyTo(uvs, offset);
        offset += 6;
    }
}

[Tool]
[GlobalClass]
public partial class ExtrudePointsMesh : ArrayMesh
{
    /* Private variables */
    protected Vector3[] BakedVertices = [];
    protected Vector3[] BakedNormals = [];
    protected Vector2[] BakedUVs = [];
    protected SurfaceTool Surface = new();
    protected Vector2[] Points = [];
    protected Vector3 Direction = Vector3.Up;
    protected float Length = 1.0f;
    protected bool Flip = false;
    protected bool SmoothNormals = true;
    protected Transform3D CustomTransform = Transform3D.Identity;
    protected BaseMaterial3D? Material;

    /* Public variables */
    [Export]
    private Vector2[] points
    {
        get => Points;
        set
        {
            Points = value;
            generate();
        }
    }

    [Export]
    private Vector3 direction
    {
        get => Direction;
        set
        {
            Direction = value;
            generate();
        }
    }

    [Export]
    private float length
    {
        get => Length;
        set
        {
            Length = value;
            generate();
        }
    }

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
    private bool smooth_normals
    {
        get => SmoothNormals;
        set
        {
            SmoothNormals = value;
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
    private BaseMaterial3D material
    {
        get => Material;
        set
        {
            Material = value;
            generate();
        }
    }

    /* Private methods */

    protected void ResizeMeshArrays(int size)
    {
        BakedVertices = new Vector3[size];
        BakedNormals = new Vector3[size];
        BakedUVs = new Vector2[size];
    }

    protected virtual Segment GetSegment(Vector2 a, Vector2 b) => (new Vector3(a.X, 0, a.Y), new Vector3(b.X, 0, b.Y));

    private void BakeSegment(Vector2 a, Vector2 b, float offset, ref int vertexOffset, ref int normalOffset, ref int uvOffset)
    {
        var segment = GetSegment(a, b);
        var extruded = new ExtrudeSegment(segment, Direction, Length, offset);
        extruded.BakeVertices(ref BakedVertices, ref vertexOffset);
        extruded.BakeNormals(ref BakedNormals, ref normalOffset);
        extruded.BakeUVs(ref BakedUVs, ref uvOffset);
    }

    protected virtual void Clear()
    {
        BakedVertices = [];
        BakedNormals = [];
        BakedUVs = [];
        Surface.Clear();
        ClearSurfaces();
    }

    protected virtual bool Bake()
    {
        if (!CanBake()) return false;
        var bakedPoints = Points;
        if (Flip) bakedPoints = bakedPoints.Reverse().ToArray();
        ResizeMeshArrays(bakedPoints.Length * 6);
        float offset = 0;
        int vertexOffset = 0, normalOffset = 0, uvOffset = 0;
        for (int i = 0; i < bakedPoints.Length - 1; i += 1)
        {
            Vector2 a = bakedPoints[i], b = bakedPoints[i + 1];
            BakeSegment(a, b, offset, ref vertexOffset, ref normalOffset, ref uvOffset);
            offset += a.DistanceTo(b);
        }
        return IsBakeValid();
    }

    protected virtual bool CanBake() => (
        Surface != null
        && Points.Length >= 2
    );

    protected virtual bool IsBakeValid() => (
        Points.Length >= 2
        && BakedVertices.Length >= 3
        && BakedVertices.Length % 3 == 0
        && BakedNormals.Length == BakedVertices.Length
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
            if (!SmoothNormals && BakedNormals.Length > i)
                Surface.SetNormal((BakedNormals[i] * CustomTransform.AffineInverse()).Normalized());
            Surface.AddVertex(BakedVertices[i] * CustomTransform.AffineInverse());
        }
        if (SmoothNormals) Surface.GenerateNormals();
        Surface.GenerateTangents();
        Surface.Commit(this);
        if (GetSurfaceCount() > 0) SurfaceSetMaterial(0, Material);
        EmitChanged();
    }
}