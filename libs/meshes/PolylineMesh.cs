using System;
using System.Linq;
using Godot;

[Tool]
[GlobalClass]
public partial class PolylineMesh : CurveMesh
{
    protected float Thickness = 0.1f;

    [Export]
    private float thickness
    {
        get => Thickness;
        set
        {
            Thickness = value;
            generate();
        }
    }

    protected override bool Bake()
    {
        if (!BakePoints()) return false;
        var bakedPoints = BakedPoints;
        var offsetPolygon = BakedPoints.Grow(Thickness);
        if (Flip) bakedPoints = bakedPoints.Reverse().ToArray();
        for (int i = 1; i < bakedPoints.Length; i++)
        {
            var point = bakedPoints[i];
            var prevPoint = bakedPoints[i - 1];
            var innerPoint = offsetPolygon.Closest(point);
            var innerPrevPoint = offsetPolygon.Closest(prevPoint);
            var triangle1 = new Triangle(prevPoint.ToVector3(), innerPoint.ToVector3(), point.ToVector3());
            var triangle2 = new Triangle(prevPoint.ToVector3(), innerPrevPoint.ToVector3(), innerPoint.ToVector3());
            triangle1.BakeVertices(ref BakedVertices);
            triangle2.BakeVertices(ref BakedVertices);
            triangle1.BakeUVs(ref BakedUVs);
            triangle2.BakeUVs(ref BakedUVs);
        }
        return IsBakeValid();
    }
}
