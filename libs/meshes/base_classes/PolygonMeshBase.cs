using Godot;

#nullable enable

/// <summary>
/// Base class for generating polygon meshes based on sets of points.
/// </summary>
[Tool]
public partial class PolygonMeshBase : PackedVector2ArrayMesh
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

    internal override bool _BakePoints() => Points.Length >= 2;

    internal override Triangle[] _GetTriangles()
    {
        var indices = Geometry2D.TriangulatePolygon(Points);
        Triangle[] triangles = new Triangle[indices.Length / 3];
        for (int i = 0; i < indices.Length - 2; i += 3)
        {
            var triangle = new Triangle(
                Points[indices[i]].ToVector3(),
                Points[indices[i + 1]].ToVector3(),
                Points[indices[i + 2]].ToVector3(),
                null,
                (
                    Points[indices[i]],
                    Points[indices[i + 1]],
                    Points[indices[i + 2]]
                )
            );
            triangles[i / 3] = triangle;
        }
        return triangles;
    }
}
