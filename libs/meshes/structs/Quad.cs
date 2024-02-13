using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public struct Quad2D : IMeshComponent
{
    public const float MiterLimit = 2;

    public bool Invert { get; set; }
    public int Surface { get; set; }

    public Vector2 TopLeft;
    public Vector2 TopRight;
    public Vector2 BottomLeft;
    public Vector2 BottomRight;
    public float OffsetFromLineStart;
    public Vector3.Axis ProjectionAxis;
    public Vector3 ExtrudeDirection;
    public float ExtrudeAmount;

    public readonly float Thickness => TopLeft.DistanceTo(TopRight).Abs();
    public readonly Vector2 Direction => TopLeft.DirectionTo(BottomLeft);
    public readonly float ExtrudeLength => (ExtrudeDirection * ExtrudeAmount).Length().Abs();
    public readonly float Length => TopLeft.DistanceTo(BottomLeft).Abs();

    private readonly Vector2[] GetUVs()
    {
        var origin = new Vector2(OffsetFromLineStart, 0);
        var end = new Vector2(Length, ExtrudeLength);
        var tl = origin + end * new Vector2(0, 0);
        var tr = origin + end * new Vector2(1, 0);
        var br = origin + end * new Vector2(1, 1);
        var bl = origin + end * new Vector2(0, 1);
        return [br, tl, tr, tl, br, bl];
    }

    public Triangle[] GetTriangles()
    {
        var points = (
            TopRight: TopRight.ToVector3(ProjectionAxis) + ExtrudeDirection * ExtrudeAmount,
            BottomRight: BottomRight.ToVector3(ProjectionAxis) + ExtrudeDirection * ExtrudeAmount,
            TopLeft: TopLeft.ToVector3(ProjectionAxis),
            BottomLeft: BottomLeft.ToVector3(ProjectionAxis)
        );
        var uvs = GetUVs();
        var triangleA = new Triangle(points.TopLeft, points.BottomRight, points.TopRight, customUVs: (uvs[0], uvs[1], uvs[2]), inverted: Invert, surface: Surface);
        var triangleB = new Triangle(points.BottomRight, points.TopLeft, points.BottomLeft, customUVs: (uvs[3], uvs[4], uvs[5]), inverted: Invert, surface: Surface);
        return [triangleA, triangleB];
    }

    public Vector2[] GetPolygon() => [TopLeft, TopRight, BottomRight, BottomLeft];

    public static bool Join(ref Quad2D a, ref Quad2D b)
    {
        try
        {
            var intersectionA = b.TopRight.Intersect(b.Direction, a.BottomRight, a.Direction, MiterLimit);
            b.TopRight = intersectionA;
            a.BottomRight = intersectionA;

            var intersectionB = b.TopLeft.Intersect(b.Direction, a.BottomLeft, a.Direction, MiterLimit);
            b.TopLeft = intersectionB;
            a.BottomLeft = intersectionB;
            return true;
        }
        catch (MiterLimitReachedException)
        {
            return false;
        }
    }

    public static Quad2D CreateBevel(ref Quad2D a, ref Quad2D b)
    {
        var newQuad = a;
        newQuad.TopLeft = a.BottomLeft;
        newQuad.TopRight = a.BottomRight;
        newQuad.BottomLeft = b.TopLeft;
        newQuad.BottomRight = b.TopRight;
        return newQuad;
    }

    public static void SimulateJoinStart(ref Quad2D[] quads, Quad2D reference)
    {
        try
        {
            quads[0].TopRight = quads[0].TopRight.Intersect(quads[0].Direction, reference.BottomRight, reference.Direction, MiterLimit);
            quads[0].TopLeft = quads[0].TopLeft.Intersect(quads[0].Direction, reference.BottomLeft, reference.Direction, MiterLimit);
        }
        catch (MiterLimitReachedException)
        {
            var bevel = CreateBevel(ref quads[0], ref reference);
            quads = [bevel, .. quads];
        }
    }

    public static void SimulateJoinEnd(ref Quad2D[] quads, Quad2D reference)
    {
        try
        {
            quads[^1].BottomRight = quads[^1].BottomRight.Intersect(quads[^1].Direction, reference.TopRight, reference.Direction, MiterLimit);
            quads[^1].BottomLeft = quads[^1].BottomLeft.Intersect(quads[^1].Direction, reference.TopLeft, reference.Direction, MiterLimit);
        }
        catch (MiterLimitReachedException)
        {
            var bevel = CreateBevel(ref quads[^1], ref reference);
            quads = [.. quads, bevel];
        }
    }

    public static void Joined(ref Quad2D[] quads, bool isClosed)
    {
        Quad2D[] results = [];

        int i = 0;
        while (i < quads.Length - 1)
        {
            results = [.. results, quads[i]];
            var joinSuccess = Join(ref results[^1], ref quads[i + 1]);
            if (!joinSuccess)
            {
                var bevel = CreateBevel(ref results[^1], ref quads[i + 1]);
                Join(ref results[^1], ref bevel);
                results = [.. results, bevel];
            }
            i += 1;
        }

        if (results.Length < 2) return;

        if (isClosed)
        {
            if (!Join(ref results[^1], ref results[0]))
            {
                var bevel = CreateBevel(ref results[^1], ref results[0]);
                Join(ref results[^1], ref bevel);
                Join(ref bevel, ref results[0]);
                results = [.. results, bevel];
            }
        }
        else
        {
            results = [.. results, quads[^1]];
            // Join(ref results[^2], ref results[^1]);
        }

        quads = results;
    }

    public readonly bool Equals(Quad2D other) => (
        TopLeft.IsEqualApprox(other.TopLeft)
        && TopRight.IsEqualApprox(other.TopRight)
        && BottomLeft.IsEqualApprox(other.BottomLeft)
        && BottomRight.IsEqualApprox(other.BottomRight)
        && OffsetFromLineStart.IsEqualApprox(other.OffsetFromLineStart)
        && ExtrudeDirection.IsEqualApprox(other.ExtrudeDirection)
        && ExtrudeAmount.IsEqualApprox(other.ExtrudeAmount)
        && ProjectionAxis == other.ProjectionAxis
        && Invert == other.Invert
        && Surface == other.Surface
    );

    public readonly override string ToString() => $"Quad2D {{ TL: {TopLeft.ToPrecisionString()}, TR: {TopRight.ToPrecisionString()}, BL: {BottomLeft.ToPrecisionString()}, BR: {BottomRight.ToPrecisionString()} }}";
}