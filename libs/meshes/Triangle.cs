using Godot;

using Tri3D = (Godot.Vector3 A, Godot.Vector3 B, Godot.Vector3 C);
using Tri2D = (Godot.Vector2 A, Godot.Vector2 B, Godot.Vector2 C);
using Segment2D = (Godot.Vector2 A, Godot.Vector2 B);
using Segment3D = (Godot.Vector3 A, Godot.Vector3 B);

public struct Triangle
{
    public Tri3D Points;
    public Tri3D? CustomNormals = null;
    public Tri2D? CustomUVs = null;

    public Triangle(Vector3 a, Vector3 b, Vector3 c, Tri3D? customNormals = null, Tri2D? customUVs = null)
    {
        this.Points = (a, b, c);
        this.CustomNormals = customNormals;
        this.CustomUVs = customUVs;
    }

    public Vector3 GetNormal() => (Points.B - Points.C).Cross(Points.A - Points.C).Normalized();
    public Vector3 GetInvertedNormal() => (Points.B - Points.A).Cross(Points.C - Points.A).Normalized();

    public Triangle Inverted()
    {
        var (A, B, C) = Points;
        Points = (C, B, A);
        if (CustomUVs is Tri2D customUVs)
        {
            var (uvA, uvB, uvC) = customUVs;
            CustomUVs = (uvC, uvB, uvA);
        }
        if (CustomNormals is Tri3D customNormals)
        {
            var (normalA, normalB, normalC) = customNormals;
            CustomNormals = (normalC, normalB, normalA);
        }
        return this;
    }

    public readonly bool IsValid() => (
        Points.A.IsFinite()
        && Points.B.IsFinite()
        && Points.C.IsFinite()
    );

    public void BakeVertices(ref Vector3[] vertices, ref int offset)
    {
        if (!IsValid()) return;
        vertices[offset] = Points.A;
        vertices[offset + 1] = Points.B;
        vertices[offset + 2] = Points.C;
        offset += 3;
    }

    public void BakeNormals(ref Vector3[] normals, ref int offset)
    {
        if (!IsValid()) return;
        if (CustomNormals is Tri3D customNormals)
        {
            normals[offset] = customNormals.A;
            normals[offset + 1] = customNormals.B;
            normals[offset + 2] = customNormals.C;
        }
        else
        {
            var normal = GetNormal();
            for (int i = 0; i < 3; i++) normals[offset + i] = normal;
        }
        offset += 3;
    }

    /// <summary>
    /// Bakes in world coordinates. Assumes the normal is pointed up, will be updated later.
    /// </summary>
    public void BakeUVs(ref Vector2[] uvs, ref int offset)
    {
        if (!IsValid()) return;
        if (CustomUVs is Tri2D customUVs)
        {
            uvs[offset] = customUVs.A;
            uvs[offset + 1] = customUVs.B;
            uvs[offset + 2] = customUVs.C;
        }
        else
        {
            uvs[offset] = Points.A.FromVector3();
            uvs[offset + 1] = Points.B.FromVector3();
            uvs[offset + 2] = Points.C.FromVector3();
        }
        offset += 3;
    }

    public override string ToString() => $"Triangle({Points.A}, {Points.B}, {Points.C})";
}

public struct Quad
{
    public Vector2[] Points;
    public Vector2 A => Points[0];
    public Vector2 B => Points[1];
    public Vector2 C => Points[2];
    public Vector2 D => Points[3];

    public Quad() => this.Points = [Vector2.Inf, Vector2.Inf, Vector2.Inf, Vector2.Inf];
    public Quad(Vector2[] points) => this.Points = points;

    public (Triangle A, Triangle B) GetTriangles()
    {
        var points = (
            A: Points[0].ToVector3(),
            B: Points[1].ToVector3(),
            C: Points[2].ToVector3(),
            D: Points[3].ToVector3()
        );
        var triangleA = new Triangle(points.A, points.B, points.C);
        var triangleB = new Triangle(points.D, points.C, points.B);
        return (triangleA, triangleB);
    }

    public override string ToString() => $"Quad({Points[0]}, {Points[1]}, {Points[2]}, {Points[3]})";
}

public struct Segment
{
    public Segment3D Points;
    public float Offset;
    public Vector3 Direction;
    public float Length;
    public readonly float SegmentLength => Points.A.DistanceTo(Points.B);

    internal Vector2[] GetUVs()
    {
        var o = new Vector2(Offset, 0);
        var e = new Vector2(SegmentLength, Direction.Length() * Length);
        var tl = o + e * new Vector2(0, 0);
        var tr = o + e * new Vector2(1, 0);
        var br = o + e * new Vector2(1, 1);
        var bl = o + e * new Vector2(0, 1);
        return [tl, br, bl, br, tl, tr];
    }

    public Segment(Segment3D points, Vector3 direction, float length, float offset)
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
        var uvs = GetUVs();
        var triangleA = new Triangle(points.C, points.B, points.A);
        var triangleB = new Triangle(points.B, points.C, points.D);
        triangleA.CustomUVs = (uvs[0], uvs[1], uvs[2]);
        triangleB.CustomUVs = (uvs[3], uvs[4], uvs[5]);
        return (triangleA, triangleB);
    }
}
