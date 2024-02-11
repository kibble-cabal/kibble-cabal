using Godot;

public struct Quad : IMeshComponent
{
    public bool Invert { get; set; }
    public int Surface { get; set; }
    public Vector2 TopRight = Vector2.Zero;
    public Vector2 BottomRight = Vector2.Zero;
    public Vector2 TopLeft = Vector2.Zero;
    public Vector2 BottomLeft = Vector2.Zero;
    public Vector3 Offset = Vector3.Zero;
    public Vector3.Axis ZeroAxis = Vector3.Axis.Y;

    public Quad() { }

    public readonly Triangle[] GetTriangles()
    {
        var points = (
            TopRight: TopRight.ToVector3(ZeroAxis) + Offset,
            BottomRight: BottomRight.ToVector3(ZeroAxis) + Offset,
            TopLeft: TopLeft.ToVector3(ZeroAxis) + Offset,
            BottomLeft: BottomLeft.ToVector3(ZeroAxis) + Offset
        );
        var triangleA = new Triangle(points.TopRight, points.BottomRight, points.TopLeft, inverted: Invert, surface: Surface);
        var triangleB = new Triangle(points.BottomLeft, points.TopLeft, points.BottomRight, inverted: Invert, surface: Surface);
        return [triangleA, triangleB];
    }

    public Quad SimulateJoined(Vector2 prevDirection, Vector2 prevBottomRight, Vector2 prevBottomLeft)
    {
        var dir = TopRight.DirectionTo(BottomRight);
        var intersectionA = TopRight.Intersect(dir, prevBottomRight, prevDirection);
        var intersectionB = TopLeft.Intersect(dir, prevBottomLeft, prevDirection);
        TopRight = intersectionA;
        TopLeft = intersectionB;
        return this;
    }

    public (Quad Previous, Quad Next) Joined(Quad previous)
    {
        var dir = TopRight.DirectionTo(BottomRight);
        var prevDir = previous.TopRight.DirectionTo(previous.BottomRight);
        var intersectionA = TopRight.Intersect(dir, previous.BottomRight, prevDir);
        var intersectionB = TopLeft.Intersect(dir, previous.BottomLeft, prevDir);
        TopRight = intersectionA;
        previous.BottomRight = intersectionA;
        TopLeft = intersectionB;
        previous.BottomLeft = intersectionB;
        return (previous, this);
    }

    public override readonly string ToString() => $"Quad[TL: {TopLeft}, TR: {TopRight}, BL: {BottomLeft}, BR: {BottomRight})";
}

public static class QuadExtensions
{
    public static void Join(this Quad[] quads, bool isClosed)
    {
        for (int i = 0; i < quads.Length - 1; i++)
        {
            var (prev, next) = quads[i + 1].Joined(quads[i]);
            quads[i] = prev;
            quads[i + 1] = next;
        }
        if (isClosed && quads.Length >= 2)
        {
            var (prev, next) = quads[0].Joined(quads[^1]);
            quads[0] = next;
            quads[^1] = prev;
        }
    }
}