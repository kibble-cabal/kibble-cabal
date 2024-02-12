using Godot;

using Tri3D = (Godot.Vector3 A, Godot.Vector3 B, Godot.Vector3 C);
using Tri2D = (Godot.Vector2 A, Godot.Vector2 B, Godot.Vector2 C);
using System.Linq;

public struct Triangle : IMeshComponent
{
    public bool Invert { get; set; }
    public int Surface { get; set; }
    public Tri3D Points;
    public Tri3D? CustomNormals = null;
    public Tri2D? CustomUVs = null;

    public Triangle(Vector3 a, Vector3 b, Vector3 c, Tri3D? customNormals = null, Tri2D? customUVs = null, int surface = 0, bool inverted = false)
    {
        this.Points = (a, b, c);
        this.CustomNormals = customNormals;
        this.CustomUVs = customUVs;
        this.Surface = surface;
        if (inverted) this = Inverted();
    }

    public readonly Vector3 GetNormal() => (Points.B - Points.C).Cross(Points.A - Points.C).Normalized();
    public readonly Vector3 GetInvertedNormal() => (Points.B - Points.A).Cross(Points.C - Points.A).Normalized();

    public Triangle Inverted()
    {
        var (A, B, C) = Points;
        Points = (C, B, A);
        if (CustomUVs is Tri2D customUVs)
            CustomUVs = (customUVs.C, customUVs.B, customUVs.A);
        if (CustomNormals is Tri3D customNormals)
            CustomNormals = (customNormals.C, customNormals.B, customNormals.A);
        Invert = !Invert;
        return this;
    }

    public readonly bool IsValid() => (
        Points.A.IsFinite()
        && Points.B.IsFinite()
        && Points.C.IsFinite()
    );

    public readonly void BakeVertices(ref Vector3[] vertices, ref int offset)
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
            normals[offset] = customNormals.A.Normalized();
            normals[offset + 1] = customNormals.B.Normalized();
            normals[offset + 2] = customNormals.C.Normalized();
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
    public readonly void BakeUVs(ref Vector2[] uvs, ref int offset)
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
            uvs[offset] = Points.A.ToVector2();
            uvs[offset + 1] = Points.B.ToVector2();
            uvs[offset + 2] = Points.C.ToVector2();
        }
        offset += 3;
    }

    public override readonly string ToString() => $"Triangle({Points.A}, {Points.B}, {Points.C})";

    public readonly Triangle[] GetTriangles() => [this];

    public static Triangle operator *(Triangle triangle, Transform3D transform)
    {
        triangle.Points.A *= transform;
        triangle.Points.B *= transform;
        triangle.Points.C *= transform;
        if (triangle.CustomNormals is Tri3D customNormals)
            triangle.CustomNormals = (
                (customNormals.A * transform).Normalized(),
                (customNormals.B * transform).Normalized(),
                (customNormals.C * transform).Normalized()
            );
        return triangle;
    }
}


public static class TriangleExtension
{
    public static Triangle[] Inverted(this Triangle[] triangles)
    {
        return triangles.Select(triangle => triangle.Inverted()).ToArray();
    }
}