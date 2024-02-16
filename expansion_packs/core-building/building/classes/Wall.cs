using System.Collections.Generic;
using System.Linq;
using Godot;

using Array = Godot.Collections.Array;

public class Wall : IGodotSerializable<Wall>, IBuildingComponent<Wall>
{
    public MaterialMap Materials { get; set; }

    public const float DefaultHeight = 2.0f;
    public const float DefaultThickness = 0.1f;

    public Vector2 Start = Vector2.Inf;
    public Vector2 StartHandle = Vector2.Zero;
    public Vector2 End = Vector2.Inf;
    public Vector2 EndHandle = Vector2.Zero;
    public float Height = DefaultHeight;
    public float Thickness = DefaultThickness;

    public Wall(Vector2[] points) => this.AddPoints(points);

    public Wall(Vector2 start, Vector2 end, Vector2? startHandle = null, Vector2? endHandle = null)
    {
        this.Start = start;
        this.StartHandle = startHandle ?? Vector2.Zero;
        this.End = end;
        this.EndHandle = endHandle ?? Vector2.Zero;
    }

    public Wall() { }

    public StringName InteriorID
    {
        get => Materials.ContainsKey("interior") ? Materials["interior"] : new StringName();
        set => Materials.Add("interior", value);
    }

    public StringName ExteriorID
    {
        get => Materials.ContainsKey("exterior") ? Materials["exterior"] : new StringName();
        set => Materials.Add("exterior", value);
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

    public void MoveBy(Vector2 delta)
    {
        Start += delta;
        End += delta;
    }

    public Rect2 GetBoundingBox() => Tessellate().GetBoundingBox();

    public Vector2 GetMidpoint() => Sample(0.5f);

    public int GetIndex(Building building) => building.Walls.IndexOf(this);

    public bool IsValid() => Start.IsFinite() && End.IsFinite() && StartHandle.IsFinite() && EndHandle.IsFinite();

    public Vector2[] Tessellate() => Tessellator.tessellate(Start, End, StartHandle, EndHandle, Building.TessellationStages, Building.TessellationToleranceDegrees);

    public bool IsTouching(Wall other, float threshold = F.AlmostZero)
    {
        if (!other.IsValid()) return false;
        (Vector2 a, Vector2 b)[] pairs = [
            (Start, other.Start),
            (Start, other.End),
            (End, other.Start),
            (End, other.End)
        ];
        return pairs.Any(pair => pair.a.DistanceTo(pair.b).Abs() < threshold);
    }

    public IEnumerable<int> GetTouching(Building building) => building.Walls
        .Where(wall => wall != this && wall.IsValid())
        .Select(wall => IsTouching(wall) ? wall : null)
        .WhereNotNull()
        .Select(wall => wall.GetIndex(building));

    public Vector2? GetJoin(Building building, Vector2 position)
    {
        foreach (var other in GetTouching(building).Select(building.Get<Wall>).WhereNotNull())
        {
            var otherPoints = other.Tessellate();
            if (otherPoints.Length < 2) continue;
            if (position.DistanceTo(otherPoints[0]).Abs() < F.AlmostZero) return otherPoints[1];
            if (position.DistanceTo(otherPoints[^1]).Abs() < F.AlmostZero) return otherPoints[^2];
        }
        return null;
    }

    public Vector2 Sample(float offset) => Start.BezierInterpolate(Start + StartHandle, End + EndHandle, End, offset);
    public Vector2 ClosestPoint(Vector2 position) => position.Closest(Start, End);
    public Vector2 ClosestPointOnSurface(Vector2 toPoint) => Tessellator.ClosestPointToBezierCurve(toPoint, Start, End, StartHandle, EndHandle, 0.05f);

    private StandardMaterial3D MakeMaterial(float r, float g, float b) => new StandardMaterial3D { AlbedoColor = new Color(r, g, b) };

    public Mesh[] GenerateMeshes(Building building)
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
        return [new Godot.Collections.Dictionary<string, Variant> {
            { "positions", positions },
            { "thickness", Thickness },
            { "height", Height },
            { "materials", Materials }
        }];
    }
    public static Result<Wall, GodotSerializationError> Deserialize(Array data) => Result.FromException(
        () =>
        {
            var values = data[0].As<Godot.Collections.Dictionary<string, Variant>>();
            Wall wall = new(values["positions"].As<Vector2[]>())
            {
                Thickness = values["thickness"].As<float>(),
                Height = values["height"].As<float>(),
                Materials = values["materials"]
            };
            return wall;
        },
        onError: _ => GodotSerializationError.IncorrectData
    );

    public bool Equals(Wall other) => GetHashCode() == other.GetHashCode();
}