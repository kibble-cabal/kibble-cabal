using Godot;

#nullable enable

/// <summary>
/// Base class for generating polygon meshes based on sets of points.
/// </summary>
[Tool]
public abstract partial class PolygonMeshBase : PackedVector2ArrayMesh
{
    /* Private variables */
    protected BaseMaterial3D? Material;

    /* Public variables */

    [Export]
    public BaseMaterial3D? material
    {
        get => Material;
        set
        {
            Material = value;
            if (Material is Material mat)
                InternalMesh.OverrideMaterials = [mat];
            InternalMesh.Generate(this);
        }
    }

    /* Private methods */

    protected Polygon GetPolygon() => new Polygon { Points = Points, Invert = Invert };

    protected override Vector2[] _BakePoints() => Points;
    protected override IMeshComponent[] _GetComponents() => [GetPolygon()];
    // protected override Triangle[] _GetTriangles() => GetPolygon().GetTriangles();
}
