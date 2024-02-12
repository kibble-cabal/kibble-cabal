using Godot;

public struct Line : IMeshComponent
{
    public bool Invert { get; set; }
    public int Surface { get; set; }
    public Vector2[] Points;
    public bool Flat;
    public Vector3.Axis ProjectionAxis;
    public Vector3 ExtrudeDirection;
    public float ExtrudeAmount;
    public Transform3D CustomTransform;

    public bool IsClosed() => Points.Length >= 3 && Points[0].IsEqualApprox(Points[^1]);

    public Line OffsetBy(float amount)
    {
        Points = Points.OffsetBy(amount, IsClosed());
        return this;
    }

    public Quad2D[] GetQuads()
    {
        Quad2D[] quads = new Quad2D[Points.Length - 1];
        float offsetFromLineStart = 0.0f;
        for (int i = 0; i < Points.Length - 1; i++)
        {
            Vector2 p1 = Points[i], p2 = Points[i + 1];
            if (Flat)
            {
                var amount = new Vector2(ExtrudeAmount, ExtrudeAmount) / 2;
                Vector2 rotation = amount.Rotated(p1.AngleToPoint(p2));
                quads[i] = new Quad2D
                {
                    TopLeft = p1 - rotation,
                    TopRight = p1 + rotation,
                    BottomLeft = p2 - rotation,
                    BottomRight = p2 + rotation,
                    ProjectionAxis = ProjectionAxis,
                    ExtrudeAmount = 0,
                    ExtrudeDirection = Vector3.Zero,
                    OffsetFromLineStart = offsetFromLineStart,
                    Invert = Invert,
                    Surface = Surface
                };
            }
            else quads[i] = new Quad2D
            {
                TopLeft = p1,
                TopRight = p1,
                BottomLeft = p2,
                BottomRight = p2,
                ProjectionAxis = ProjectionAxis,
                ExtrudeAmount = ExtrudeAmount,
                ExtrudeDirection = ExtrudeDirection,
                OffsetFromLineStart = offsetFromLineStart,
                Invert = Invert,
                Surface = Surface
            };
            offsetFromLineStart += quads[i].Length;
        }
        Quad2D.Join(ref quads, IsClosed());
        return quads;
    }

    public Triangle[] GetTriangles()
    {
        Quad2D[] quads = GetQuads();
        Triangle[] triangles = new Triangle[(Points.Length - 1) * 2];
        for (int i = 0; i < Points.Length - 1; i++)
        {
            var tris = quads[i].GetTriangles();
            triangles[i * 2] = tris[0] * CustomTransform.AffineInverse();
            triangles[i * 2 + 1] = tris[1] * CustomTransform.AffineInverse();
        }
        return triangles;
    }
}