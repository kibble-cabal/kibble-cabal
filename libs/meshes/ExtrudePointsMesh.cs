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

    public void BakeVertices(ref Array<Vector3> vertices) => vertices.AddRange([Points.A, Points.B, Points.C]);
    public void BakeNormals(ref Array<Vector3> normals)
    {
        var normal = GetNormal();
        normals.AddRange([normal, normal, normal]);
    }
    /// <summary>
    /// Bakes in world coordinates. Assumes the normal is pointed up, will be updated later.
    /// </summary>
    public void BakeUVs(ref Array<Vector2> uvs) => uvs.AddRange([Points.A.FromVector3(), Points.B.FromVector3(), Points.C.FromVector3()]);
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

    public void BakeVertices(ref Array<Vector3> vertices)
    {
        var (A, B) = GetTriangles();
        A.BakeVertices(ref vertices);
        B.BakeVertices(ref vertices);
    }
    public void BakeNormals(ref Array<Vector3> normals)
    {
        var (A, B) = GetTriangles();
        A.BakeNormals(ref normals);
        B.BakeNormals(ref normals);
    }

    public void BakeUVs(ref Array<Vector2> uvs)
    {
        var o = new Vector2(Offset, 0);
        var e = new Vector2(SegmentLength, Direction.Length() * Length);
        var tl = o + e * new Vector2(0, 0);
        var tr = o + e * new Vector2(1, 0);
        var br = o + e * new Vector2(1, 1);
        var bl = o + e * new Vector2(0, 1);
        uvs.AddRange([tl, br, bl]);
        uvs.AddRange([br, tl, tr]);
    }
}

[Tool]
[GlobalClass]
public partial class ExtrudePointsMesh : ArrayMesh
{
    /* Private variables */
    protected Array<Vector3> BakedVertices = [];
    protected Array<Vector3> BakedNormals = [];
    protected Array<Vector2> BakedUVs = [];
    protected SurfaceTool Surface = new();
    protected Vector2[] Points = [];
    protected Vector3 Direction = Vector3.Up;
    protected float Length = 1.0f;
    protected bool Flip = false;
    protected bool SmoothNormals = true;

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

    /* Private methods */

    protected virtual Segment GetSegment(Vector2 a, Vector2 b) => (new Vector3(a.X, 0, a.Y), new Vector3(b.X, 0, b.Y));

    private void BakeSegment(Vector2 a, Vector2 b, float offset)
    {
        var segment = GetSegment(a, b);
        var extruded = new ExtrudeSegment(segment, Direction, Length, offset);
        extruded.BakeVertices(ref BakedVertices);
        extruded.BakeNormals(ref BakedNormals);
        extruded.BakeUVs(ref BakedUVs);
    }

    protected virtual void Clear()
    {
        BakedVertices.Clear();
        BakedNormals.Clear();
        BakedUVs.Clear();
        Surface.Clear();
        ClearSurfaces();
    }

    protected virtual bool Bake()
    {
        if (!CanBake()) return false;
        var bakedPoints = Points;
        if (Flip) bakedPoints = bakedPoints.Reverse().ToArray();
        float offset = 0;
        for (int i = 0; i < bakedPoints.Length - 1; i += 1)
        {
            Vector2 a = bakedPoints[i], b = bakedPoints[i + 1];
            BakeSegment(a, b, offset);
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
        && BakedVertices.Count >= 3
        && BakedVertices.Count % 3 == 0
        && BakedNormals.Count == BakedVertices.Count
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
            if (!SmoothNormals && BakedNormals.Count > i)
                Surface.SetNormal(BakedNormals[i]);
            Surface.AddVertex(BakedVertices[i]);
        }
        if (SmoothNormals) Surface.GenerateNormals();
        Surface.GenerateTangents();
        Surface.Commit(this);
        EmitChanged();
    }
}