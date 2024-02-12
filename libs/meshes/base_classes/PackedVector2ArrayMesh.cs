using System.Linq;
using Godot;

#nullable enable


/// <summary>
/// Base class for generating meshes.
/// </summary>
[Tool]
public abstract partial class PackedVector2ArrayMesh : ArrayMesh
{
    /* Private variables */
    protected bool Invert = false;
    protected ProceduralMesh InternalMesh;
    protected Vector2[] Points = [];
    protected BaseMaterial3D[] Materials = [];

    [Export]
    public bool flip
    {
        get => Invert;
        set
        {
            Invert = value;
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
    public BaseMaterial3D[] materials
    {
        get => Materials;
        set
        {
            Materials = value;
            InternalMesh.OverrideMaterials = Materials;
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

    protected bool IsClosed()
    {
        if (Points.Length >= 3) return Points[0].IsEqualApprox(Points[^1]);
        return false;
    }

    protected IMeshComponent[] GetComponents()
    {
        if (!_CanBakePoints()) return [];
        Points = _BakePoints();
        if (!_ArePointsValid()) return [];
        return _GetComponents().Select(component =>
        {
            component.Invert = Invert;
            return component;
        }).ToArray();
    }

    /* Virtual methods */
    protected virtual bool _CanBakePoints() => true;
    protected virtual Vector2[] _BakePoints() => Points;
    protected virtual bool _ArePointsValid() => Points.Length >= 2 && Points.All(point => point.IsFinite());
    protected virtual IMeshComponent[] _GetComponents() => [];

    /* Public Methods */

    public PackedVector2ArrayMesh()
    {
        this.InternalMesh = new(GetComponents, this);
    }
}
