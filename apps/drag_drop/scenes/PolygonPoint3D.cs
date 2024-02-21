using System;
using Godot;
using Godot.Collections;

[Tool]
public partial class PolygonPoint3D : Sprite3D
{
    public static readonly PackedScene Scene = GD.Load<PackedScene>("res://apps/drag_drop/scenes/polygon_point_3d.tscn");

    [Signal]
    public delegate void PositionChangedEventHandler(Vector2 position);

    private float _inputMargin = 0.05f;
    private Draggable3D.Mode _dropMode = Draggable3D.Mode.Anywhere;
    private Array<NodePath> _dropAreas = [];
    private float _size = 0.5f;
    private Color _inactiveModulate = Colors.White;
    private Color _draggingModulate = Colors.White;

    [Export]
    public int Index;

    [ExportGroup("Behavior")]

    [Export]
    public float InputMargin
    {
        get => _inputMargin;
        set
        {
            _inputMargin = value;
            Update();
        }
    }

    [Export]
    public Draggable3D.Mode DropMode
    {
        get => _dropMode;
        set
        {
            _dropMode = value;
            Update();
        }
    }

    [Export]
    public Array<NodePath> DropAreas
    {
        get => _dropAreas;
        set
        {
            _dropAreas = value;
            Update();
        }
    }

    [ExportGroup("Appearance")]

    [Export]
    public float Size
    {
        get => _size;
        set
        {
            _size = value;
            Update();
        }
    }

    [Export]
    public Color InactiveModulate
    {
        get => _inactiveModulate;
        set
        {
            _inactiveModulate = value;
            Update();
        }
    }

    [Export]
    public Color DraggingModulate
    {
        get => _draggingModulate;
        set
        {
            _draggingModulate = value;
            Update();
        }
    }

    public History? History;
    public Func<Vector2, Vector2>? CustomSnapMethod;

    private Draggable3D? Draggable;
    private CollisionShape3D? Collider;

    public override void _Ready()
    {
        Draggable = GetNode<Draggable3D>("Draggable");
        Collider = GetNode<CollisionShape3D>("Draggable/CollisionShape");
        Modulate = InactiveModulate;
        if (Draggable is null) return;
        Draggable.DragStarted += OnDragStarted;
        Draggable.DragFinished += OnDragFinished;
        Draggable.PositionChanged += OnPositionChanged;
        Update();
    }

    public void Update()
    {
        PixelSize = GetPixelSize();
        if (Collider is not null && Collider.Shape is SphereShape3D shape)
            shape.Radius = Size + InputMargin;
        if (Engine.IsEditorHint()) Modulate = InactiveModulate;
        if (Draggable is null) return;
        Draggable.DropMode = DropMode;
        Draggable.DropAreas = DropAreas;
        Draggable.StartPosition = Position;
        if (CustomSnapMethod is not null)
            Draggable.CustomSnapMethod = pos => CustomSnapMethod(pos.ToVector2()).ToVector3();
        else Draggable.CustomSnapMethod = null;
    }

    private float GetPixelSize()
    {
        if (Texture is null) return 0.01f;
        var textureSize = Texture.GetSize();
        return Size / (textureSize.X + textureSize.Y) / 2;
    }

    private void Move(Vector3 position)
    {
        GlobalPosition = position;
        EmitSignal(SignalName.PositionChanged, position.ToVector2());
    }

    private void OnDragStarted() => CreateTween().TweenProperty(this, "modulate", DraggingModulate, 0.125);
    private void OnDragFinished() => CreateTween().TweenProperty(this, "modulate", InactiveModulate, 0.125);
    private void OnPositionChanged(Vector3 newPosition, Vector3 oldPosition)
    {
        // TODO merge
        if (History is not null) History.Add(
            "Move Point",
            () => Move(newPosition),
            () => Move(oldPosition)
        );
        else Move(newPosition);
    }

    public static PolygonPoint3D Instantiate(int index)
    {
        var node = Scene.Instantiate<PolygonPoint3D>();
        node.Index = index;
        return node;
    }
}