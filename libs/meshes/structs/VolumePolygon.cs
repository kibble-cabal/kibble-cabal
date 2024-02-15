using System.Linq;
using Godot;

public struct VolumePolygon : IMeshComponent
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
    public float ExtrudeAmount;
    public Vector3 ExtrudeDirection;
    public bool RenderTop;
    public bool RenderBottom;
    public bool RenderSides;

    public IMeshComponent[] GetComponents()
    {
        if (Points.Length <= 2) return [];
        Vector2[] points = [.. Points, Points[0]];
        IMeshComponent[] components = [];
        if (RenderTop) components = [..components, new Polygon
        {
            Points = points,
            ProjectionAxis = Vector3.Axis.Y,
            CustomTransform = Transform3D.Identity.Translated(ExtrudeDirection * ExtrudeAmount).AffineInverse(),
            Invert = Invert,
            Surface = 0,
        }];
        if (RenderSides) components = [.. components, new Line
        {
            Points = points,
            ProjectionAxis = Vector3.Axis.Y,
            ExtrudeAmount = ExtrudeAmount,
            ExtrudeDirection = ExtrudeDirection,
            Flat = false,
            Invert = Geometry2D.IsPolygonClockwise(points) ? !Invert : Invert,
            Surface = 1,
            CustomTransform = Transform3D.Identity,
        }];
        if (RenderBottom) components = [..components,  new Polygon
        {
            Points = points,
            ProjectionAxis = Vector3.Axis.Y,
            CustomTransform = Transform3D.Identity,
            Invert = !Invert,
            Surface = 2,
        }];
        return components;
    }

    public Triangle[] GetTriangles() => GetComponents().SelectMany(component => component.GetTriangles()).ToArray();
}
