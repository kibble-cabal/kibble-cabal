using System;
using System.Linq;
using Godot;

using Array = Godot.Collections.Array;
using MaterialMap = Godot.Collections.Dictionary<Godot.StringName, Godot.StringName>;

public record Wall
{
    public Vector2 Start;
    public Vector2 StartHandle = Vector2.Zero;
    public Vector2 End;
    public Vector2 EndHandle = Vector2.Zero;
    public float Height = 2.0f;
    public float Thickness = 0.1f;

    public Wall(Vector2 aPosition, Vector2 startHandle, Vector2 bPosition, Vector2 endHandle)
    {
        this.Start = aPosition;
        this.StartHandle = startHandle;
        this.End = bPosition;
        this.EndHandle = endHandle;
    }

    public Wall(Vector2 aPosition, Vector2 bPosition)
    {
        this.Start = aPosition;
        this.End = bPosition;
    }

    public Wall() { }

    public MaterialMap Materials = [];

    public StringName InteriorID
    {
        get => Materials.ContainsKey("interior") ? Materials["interior"] : new StringName();
        set => Materials["interior"] = value;
    }

    public StringName ExteriorID
    {
        get => Materials.ContainsKey("exterior") ? Materials["exterior"] : new StringName();
        set => Materials["exterior"] = value;
    }

    private void AddPoints(Vector2[] points)
    {
        if (points.Length == 4)
        {
            Start = points[0];
            StartHandle = points[1];
            End = points[2];
            EndHandle = points[3];
        }
    }

    public bool IsValid() => Start.IsFinite() && End.IsFinite();

    public Vector2[] Tessellate(int maxStages = 5, float tolerance = 4) => Tessellator.tessellate(Start, End, StartHandle, EndHandle, maxStages, tolerance);

    public bool IsTouching(Wall other)
    {
        (Vector2 a, Vector2 b)[] pairs = [
            (Start, other.Start),
            (Start, other.End),
            (End, other.Start),
            (End, other.End)
        ];
        return pairs.Any(pair => Mathf.Abs(pair.a.DistanceTo(pair.b)) < F.AlmostZero);
    }

    public int[] GetTouching(Building building) => building.Walls
        .Where(wall => wall != this)
        .Select((wall, i) => IsTouching(wall) ? i : -1)
        .Where(i => i != -1)
        .ToArray();

    public Vector2? GetJoin(Building building, Vector2 position)
    {
        var others = GetTouching(building).Select(building.get_wall).ToArray();
        if (others.Length == 0) return null;
        var otherPoints = others[0].tessellate();
        if (otherPoints.Length < 2) return null;
        if (position.DistanceTo(others[0].start).Abs() < F.AlmostZero) return otherPoints[^1];
        if (position.DistanceTo(others[0].end).Abs() < F.AlmostZero) return otherPoints[1];
        return null;
    }

    public Vector2 Sample(float offset) => Start.BezierInterpolate(StartHandle, EndHandle, End, offset);

    public Vector2 ClosestPoint(Vector2 position) => position.Closest(Start, End);
    public Vector2 ClosestPointOnSurface(Vector2 toPoint, float epsilon) => Tessellator.closest_point_to_bezier_curve(toPoint, Start, End, StartHandle, EndHandle, epsilon);
    public Vector2 ClosestPointOnSurface(Vector2 toPoint) => ClosestPointOnSurface(toPoint, 0.05f);

    public Vector2 Snap(Vector2 position, float threshold) => position.Snap(ClosestPoint(position), threshold);
    public Vector2 Snap(Vector2 position) => Snap(position, -1);

    public Vector2 SnapToSurface(Vector2 position, float threshold, float epsilon) => position.Snap(ClosestPointOnSurface(position, epsilon), threshold);
    public Vector2 SnapToSurface(Vector2 position, float threshold) => SnapToSurface(position, threshold, 0.05f);
    public Vector2 SnapToSurface(Vector2 position) => SnapToSurface(position, -1, 0.05f);

    private BaseMaterial3D MakeMaterial(float r, float g, float b) => new StandardMaterial3D { AlbedoColor = new Color(r, g, b) };

    public ArrayMesh[] GenerateMeshes(Building building, int tessellationStages = 5, float tessellationTolerance = 4)
    {
        // TODO: Materials
        var mesh = new ExtrudePointsMesh
        {
            points = Tessellate(tessellationStages, tessellationTolerance),
            thickness = Thickness,
            direction = Vector3.Up,
            length = Height,
            render_bottom = false,
            join_start = GetJoin(building, Start),
            join_end = GetJoin(building, End),
            material = [
                MakeMaterial(1, 0, 0),
                MakeMaterial(1, 1, 0),
                MakeMaterial(0, 0, 1),
            ]
        };
        return [mesh];
    }

    public Array ToData()
    {
        Vector2[] positions = [Start, StartHandle, End, EndHandle];
        return [
            positions,
            Materials
        ];
    }

    public static Wall FromData(Array data)
    {
        Wall wall = new();
        if (data.Count >= 2)
        {
            wall.AddPoints(data[0].As<Vector2[]>());
            wall.Materials = data[1].As<MaterialMap>();
        }
        return wall;
    }
}