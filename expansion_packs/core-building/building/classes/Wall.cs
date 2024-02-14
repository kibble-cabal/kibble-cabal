using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

using Array = Godot.Collections.Array;
using MaterialMap = Godot.Collections.Dictionary<Godot.StringName, Godot.StringName>;

public class Wall : IGodotSerializable<Wall>
{
    public Vector2 Start = Vector2.Inf;
    public Vector2 StartHandle = Vector2.Zero;
    public Vector2 End = Vector2.Inf;
    public Vector2 EndHandle = Vector2.Zero;
    public float Height = 2.0f;
    public float Thickness = 0.1f;
    public MaterialMap Materials = [];

    public Wall(Vector2 start, Vector2 startHandle, Vector2 end, Vector2 endHandle)
    {
        this.Start = start;
        this.StartHandle = startHandle;
        this.End = end;
        this.EndHandle = endHandle;
    }

    public Wall(Vector2 start, Vector2 end)
    {
        this.Start = start;
        this.End = end;
    }

    public Wall() { }

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

    public int GetIndex(Building building) => building.Walls.IndexOf(this);

    public bool IsValid() => Start.IsFinite() && End.IsFinite() && StartHandle.IsFinite() && EndHandle.IsFinite();

    public Vector2[] Tessellate() => Tessellator.tessellate(Start, End, StartHandle, EndHandle, Building.TessellationStages, Building.TessellationToleranceDegrees);

    public bool IsTouching(Wall other)
    {
        if (!other.IsValid()) return false;
        (Vector2 a, Vector2 b)[] pairs = [
            (Start, other.Start),
            (Start, other.End),
            (End, other.Start),
            (End, other.End)
        ];
        return pairs.Any(pair => pair.a.DistanceTo(pair.b).Abs() < F.AlmostZero);
    }

    public IEnumerable<int> GetTouching(Building building) => building.Walls
        .Where(wall => wall != this && wall.IsValid())
        .Select(wall => IsTouching(wall) ? wall : null)
        .WhereNotNull()
        .Select(wall => wall.GetIndex(building));

    public Vector2? GetJoin(Building building, Vector2 position)
    {
        foreach (var other in GetTouching(building).Select(building.GetWall).WhereNotNull())
        {
            var otherPoints = other.Tessellate();
            if (otherPoints.Length < 2) continue;
            if (position.DistanceTo(otherPoints[0]).Abs() < F.AlmostZero) return otherPoints[1];
            if (position.DistanceTo(otherPoints[^1]).Abs() < F.AlmostZero) return otherPoints[^2];
        }
        return null;
    }

    public Vector2 Sample(float offset) => Start.BezierInterpolate(StartHandle, EndHandle, End, offset);
    public Vector2 ClosestPoint(Vector2 position) => position.Closest(Start, End);
    public Vector2 ClosestPointOnSurface(Vector2 toPoint, float epsilon = 0.05f) => Tessellator.closest_point_to_bezier_curve(toPoint, Start, End, StartHandle, EndHandle, epsilon);
    public Vector2 Snap(Vector2 position, float threshold = -1) => position.Snap(ClosestPoint(position), threshold);
    public Vector2 SnapToSurface(Vector2 position, float threshold = -1, float epsilon = 0.05f) => position.Snap(ClosestPointOnSurface(position, epsilon), threshold);

    private BaseMaterial3D MakeMaterial(float r, float g, float b) => new StandardMaterial3D { AlbedoColor = new Color(r, g, b) };

    public ArrayMesh[] GenerateMeshes(Building building)
    {
        // TODO: Materials
        return [new PolylinePointsMesh()
        {
            points = Tessellate(),
            extrude_thickness = Thickness,
            extrude_height = Height,
            render_bottom = false,
            join_start = GetJoin(building, Start),
            join_end = GetJoin(building, End),
            materials = [
                MakeMaterial(1, 0, 0),
                MakeMaterial(1, 1, 0),
                MakeMaterial(0, 0, 1),
                MakeMaterial(0, 1, 1)
            ],
        }];
    }

    public Array Serialize()
    {
        Vector2[] positions = [Start, StartHandle, End, EndHandle];
        return [positions, Materials];
    }

    public static Wall Deserialize(Array data)
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