using System.Linq;
using Godot;

public struct VolumePolyline : IMeshComponent
{
    enum SurfaceIndex
    {
        Outer = 0,
        Inner = 1,
        Top = 2,
        Bottom = 3,
        End = 4
    }

    public bool Invert { get; set; }
    public int Surface { get; set; }
    public Vector2[] Points;
    public float Thickness;
    public Vector3 ExtrudeDirection;
    public float ExtrudeAmount;
    public bool RenderTop;
    public bool RenderBottom;
    public bool RenderEnds;

    /// <summary>
    /// Optional. If provided, simulated joining to the given point, as if it were the first point provided.
    /// </summary>
    public Vector2? JoinStart;
    /// <summary>
    /// Optional. If provided, simulated joining to the given point, as if were the last point provided.
    /// </summary>
    public Vector2? JoinEnd;

    public readonly Vector2 ThicknessVector => Thickness.ToVector2() / 2;

    public readonly bool IsClosed() => Points.Length >= 3 && Points[0].IsEqualApprox(Points[^1]) && JoinStart == null && JoinEnd == null;

    public IMeshComponent[] GetComponents()
    {
        var topLine = new Line
        {
            Points = Points,
            ProjectionAxis = Vector3.Axis.Y,
            ExtrudeDirection = Vector3.Zero,
            ExtrudeAmount = Thickness,
            Flat = true,
            Invert = Invert,
            Surface = 2,
            CustomTransform = Transform3D.Identity.Translated(new Vector3(0, ExtrudeAmount, 0)),
            JoinStart = JoinStart,
            JoinEnd = JoinEnd
        };
        Line outerLine = new Line
        {
            Points = topLine.GetFlatInnerPolygon(),
            ProjectionAxis = Vector3.Axis.Y,
            ExtrudeAmount = ExtrudeAmount,
            ExtrudeDirection = ExtrudeDirection,
            Flat = false,
            Invert = Invert,
            Surface = 0,
            CustomTransform = Transform3D.Identity,
            JoinStart = JoinStart,
            JoinEnd = JoinEnd
        };
        Line innerLine = new Line
        {
            Points = topLine.GetFlatOuterPolygon(),
            ProjectionAxis = Vector3.Axis.Y,
            ExtrudeAmount = ExtrudeAmount,
            ExtrudeDirection = ExtrudeDirection,
            Flat = false,
            Invert = !Invert,
            Surface = 1,
            CustomTransform = Transform3D.Identity,
            JoinStart = JoinStart,
            JoinEnd = JoinEnd
        };
        IMeshComponent[] components = [outerLine, innerLine];
        if (RenderTop) components = [.. components, topLine];
        if (RenderBottom) components = [..components,  new Line
        {
            Points = Points,
            ProjectionAxis = Vector3.Axis.Y,
            ExtrudeDirection = Vector3.Zero,
            ExtrudeAmount = Thickness,
            Flat = true,
            Invert = !Invert,
            Surface = 3,
            CustomTransform = Transform3D.Identity,
            JoinStart = JoinStart,
            JoinEnd = JoinEnd
        }];
        if (RenderEnds && !IsClosed())
            return [
                ..components,
                .. GetSideTriangles(outerLine.Points[0], innerLine.Points[0], false),
                .. GetSideTriangles(outerLine.Points[^1], innerLine.Points[^1], true)
            ];
        return components;
    }

    private readonly Triangle[] GetSideTriangles(Vector2 a, Vector2 b, bool inverted)
    {
        Triangle[] triangles = new Triangle[2];
        var lengthVector = new Vector3(0, ExtrudeAmount, 0);
        var bl = a.ToVector3();
        var br = b.ToVector3();
        var tl = bl + lengthVector;
        var tr = br + lengthVector;
        triangles[0] = new Triangle(tr, br, tl, inverted: inverted, surface: (int)SurfaceIndex.End);
        triangles[1] = new Triangle(bl, tl, br, inverted: inverted, surface: (int)SurfaceIndex.End);
        return triangles;
    }

    public Triangle[] GetTriangles() => GetComponents().SelectMany(component => component.GetTriangles()).ToArray();
}
