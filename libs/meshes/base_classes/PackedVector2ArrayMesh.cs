using System.Linq;
using Godot;

#nullable enable


/// <summary>
/// Base class for generating meshes based on sets of points.
/// </summary>
[Tool]
public abstract partial class PackedVector2ArrayMesh : ArrayMesh
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

    internal bool IsClosed()
    {
        if (Points.Length >= 3) return Points[0].IsEqualApprox(Points[^1]);
        return false;
    }

    internal Triangle[] GetTriangles()
    {
        if (!_CanBakePoints()) return [];
        Points = _BakePoints();
        if (!_ArePointsValid()) return [];
        return _GetTriangles();
    }

    internal int[] GetSurfaces() => _ArePointsValid() ? _GetSurfaces() : [];

    /* Virtual methods */
    internal virtual bool _CanBakePoints() => true;
    internal virtual Vector2[] _BakePoints() => Points;
    internal virtual bool _ArePointsValid() => Points.Length >= 2 && Points.All(point => point.IsFinite());
    internal virtual Triangle[] _GetTriangles() => [];
    internal virtual int[] _GetSurfaces() => [];

    /* Public Methods */

    public PackedVector2ArrayMesh()
    {
        this.InternalMesh = new(GetTriangles, GetSurfaces, this);
    }
}
