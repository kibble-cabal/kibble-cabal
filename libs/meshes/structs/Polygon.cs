using Godot;


public struct Polygon : IMeshComponent
{
    public bool Invert { get; set; }
    public int Surface { get; set; }
    public Vector2[] Points;
    public Vector3.Axis ProjectionAxis;
    public Transform3D CustomTransform;

    public readonly bool IsClosed() => Points.Length >= 3 && Points[0].IsEqualApprox(Points[^1]);

    public readonly Triangle[] GetTriangles()
    {
        if (Points.Length < 3) return [];
        var indices = Geometry2D.TriangulatePolygon(Points);
        Triangle[] triangles = new Triangle[indices.Length / 3];
        for (int i = 0; i < indices.Length - 2; i += 3)
        {
            var triangle = new Triangle(
                Points[indices[i]].ToVector3(ProjectionAxis),
                Points[indices[i + 1]].ToVector3(ProjectionAxis),
                Points[indices[i + 2]].ToVector3(ProjectionAxis),
                customUVs: (
                    Points[indices[i]],
                    Points[indices[i + 1]],
                    Points[indices[i + 2]]
                ),
                inverted: Invert,
                surface: Surface
            ) * CustomTransform;
            triangles[i / 3] = triangle;
        }
        return triangles;
    }
}