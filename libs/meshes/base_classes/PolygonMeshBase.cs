using Godot;

#nullable enable

/// <summary>
/// Base class for generating polygon meshes based on sets of points.
/// </summary>
[Tool]
public abstract partial class PolygonMeshBase : PackedVector2ArrayMesh
{
    /* Private variables */
    internal BaseMaterial3D? Material;

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

    internal Polygon GetPolygon() => new Polygon { Points = Points };

    internal override Vector2[] _BakePoints() => Points;
    internal override Triangle[] _GetTriangles() => GetPolygon().GetTriangles();
}
