using System;
using System.Linq;
using Godot;

#nullable enable

[Tool]
[GlobalClass]
public partial class CurveMesh : PolygonMesh
{
    /* Private variables */
    protected Curve2D? Curve;
    protected int TessellationStages = 3;
    protected float TessellationToleranceDegrees = 4;

    /* Public variables */

    [Export]
    private Curve2D? curve
    {
        get => Curve;
        set
        {
            Curve?.TryDisconnectChanged(new Callable(this, "_Generate"));
            value?.TryConnectChanged(new Callable(this, "_Generate"));
            Curve = value;
            InternalMesh.Generate(this);
        }
    }

    [Export]
    private int tessellation_stages
    {
        get => TessellationStages;
        set
        {
            TessellationStages = value;
            InternalMesh.Generate(this);
        }
    }

    [Export]
    private float tessellation_tolerance_degrees
    {
        get => TessellationToleranceDegrees;
        set
        {
            TessellationToleranceDegrees = value;
            InternalMesh.Generate(this);
        }
    }

    public CurveMesh()
    {
        this.InternalMesh = new(GetTriangles, GetSurfaces, this);
    }

    /* Private methods */

    internal void _Generate() => InternalMesh.Generate(this);

    internal override Vector2[] _BakePoints() => Curve?.Tessellate(TessellationStages, TessellationToleranceDegrees) ?? [];

    internal override bool _CanBakePoints() => (
        Curve != null
        && Curve.PointCount >= 2
        && Curve.GetPointPositions().All(point => point.IsFinite())
    );
}