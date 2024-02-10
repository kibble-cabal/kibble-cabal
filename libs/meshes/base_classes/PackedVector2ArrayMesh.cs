using Godot;

#nullable enable


/// <summary>
/// Base class for generating meshes based on sets of points.
/// </summary>
[Tool]
public partial class PackedVector2ArrayMesh : ArrayMesh
{
    /* Private variables */
    internal ProceduralMesh InternalMesh;
    protected Vector2[] Points = [];

    [Export]
    public bool flip
    {
        get => InternalMesh.FlipFaces;
        set
        {
            InternalMesh.FlipFaces = value;
            InternalMesh.Generate(this);
        }
    }

    [Export]
    public bool smooth_normals
    {
        get => InternalMesh.SmoothNormals;
        set
        {
            InternalMesh.SmoothNormals = value;
            InternalMesh.Generate(this);
        }
    }

    [Export]
    public Transform3D custom_transform
    {
        get => InternalMesh.CustomTransform;
        set
        {
            InternalMesh.CustomTransform = value;
            InternalMesh.Generate(this);
        }
    }

    internal Triangle[] GetTriangles()
    {
        if (!_BakePoints()) return [];
        return _GetTriangles();
    }

    internal virtual bool _BakePoints() => true;
    internal virtual Triangle[] _GetTriangles() => [];

    /* Public Methods */

    public PackedVector2ArrayMesh()
    {
        this.InternalMesh = new(GetTriangles, this);
    }
}
