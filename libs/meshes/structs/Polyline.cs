using Godot;

public struct Polyline : IMeshComponent
{
    public Vector2[] Points;
    public Vector3.Axis ZeroAxis;
    public Vector3 Offset;
    public float Thickness;

    public bool IsClosed() => Points.Length >= 3 && Points[0].IsEqualApprox(Points[^1]);

    public Quad[] GetQuads()
    {
        var quads = new Quad[Points.Length - 1];
        var thickness = new Vector2(Thickness, Thickness) / 2;
        var isClosed = IsClosed();
        for (int i = 0; i < Points.Length - 1; i++)
        {
            var point = Points[i];
            var nextPoint = Points[i + 1];
            var angle = point.AngleToPoint(nextPoint);
            var quad = new Quad
            {
                TopRight = point - thickness.Rotated(angle),
                BottomRight = nextPoint - thickness.Rotated(angle),
                TopLeft = point + thickness.Rotated(angle),
                BottomLeft = nextPoint + thickness.Rotated(angle),
                Offset = Offset,
                ZeroAxis = ZeroAxis,
            };

            // Account for angle at start
            if (i == 0 && !isClosed)
                quad.TopRight = quad.TopRight.MoveToward(quad.BottomRight, Thickness);

            // Account for angle at end
            if (i == Points.Length - 2 && !isClosed)
                quad.BottomLeft = quad.BottomLeft.MoveToward(quad.TopLeft, Thickness);

            quads[i] = quad;
        }
        quads.Join(isClosed);
        return quads;
    }

    public int GetTriangleCount() => Points.Length * 2;
    public Triangle[] GetTriangles()
    {
        var quads = GetQuads();
        Triangle[] triangles = new Triangle[Points.Length * 2];
        for (int i = 0; i < quads.Length; i++)
        {
            var quadTriangles = quads[i].GetTriangles();
            triangles[i * 2] = quadTriangles[0];
            triangles[i * 2 + 1] = quadTriangles[1];
        }
        return triangles;
    }

}