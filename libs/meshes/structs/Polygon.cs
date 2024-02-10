using Godot;


public struct Polygon : IMeshComponent
{
    public Vector2[] Points;

    public bool IsClosed() => Points.Length >= 3 && Points[0].IsEqualApprox(Points[^1]);

    public int GetTriangleCount() => Geometry2D.TriangulatePolygon(Points).Length / 3;
    public Triangle[] GetTriangles()
    {
        if (Points.Length < 3) return [];
        var indices = Geometry2D.TriangulatePolygon(Points);
        Triangle[] triangles = new Triangle[indices.Length / 3];
        for (int i = 0; i < indices.Length - 2; i += 3)
        {
            var triangle = new Triangle(
                Points[indices[i]].ToVector3(),
                Points[indices[i + 1]].ToVector3(),
                Points[indices[i + 2]].ToVector3(),
                null,
                (
                    Points[indices[i]],
                    Points[indices[i + 1]],
                    Points[indices[i + 2]]
                )
            );
            triangles[i / 3] = triangle;
        }
        return triangles;
    }
}