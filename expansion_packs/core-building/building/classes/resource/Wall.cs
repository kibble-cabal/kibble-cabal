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

    public bool IsTouching(Wall other, float threshold)
    {
        (Vector2 a, Vector2 b)[] pairs = [
            (Start, other.Start),
            (Start, other.End),
            (End, other.Start),
            (End, other.End)
        ];
        return pairs.Any((pair) => Mathf.Abs(pair.a.DistanceTo(pair.b)) < threshold);
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