using Godot;

#nullable enable

/// <summary>
/// Base class for generating extruded meshes based on sets of points.
/// </summary>
[Tool]
public abstract partial class ExtrudePackedVector2ArrayMesh : PackedVector2ArrayMesh
{
    /* Private variables */
    protected Vector3 Direction = Vector3.Up;
    protected float Length = 1.0f;
    protected BaseMaterial3D[] Materials = [];

    /* Public variables */

    [Export]
    public Vector3 direction
    {
        get => Direction;
        set
        {
            Direction = value;
            InternalMesh.Generate(this);
        }
    }

    [Export]
    public float length
    {
        get => Length;
        set
        {
            Length = value;
            InternalMesh.Generate(this);
        }
    }

    [Export]
    public BaseMaterial3D[] material
    {
        get => Materials;
        set
        {
            Materials = value;
            InternalMesh.OverrideMaterials = Materials;
            InternalMesh.Generate(this);
        }
    }

    /* Private methods */

    protected Segment GetSegment(Vector2 a, Vector2 b, float offsetFromStart) => new Segment
    {
        Points = (a, b),
        Direction = Direction,
        Length = Length,
        Offset = offsetFromStart
    };

    protected override IMeshComponent[] _GetComponents()
    {
        IMeshComponent[] segments = new IMeshComponent[Points.Length - 1];
        float offset = 0;
        for (int i = 0; i < Points.Length - 1; i++)
        {
            var segment = GetSegment(Points[i], Points[i + 1], offset);
            segments[i] = segment;
            offset += Points[i].DistanceTo(Points[i + 1]);
        }
        return segments;
    }
}
