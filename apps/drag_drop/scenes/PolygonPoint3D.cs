using System;
using Godot;
using Godot.Collections;
using KibbleCabal.Apps.DragDrop.UndoRedo;
using UndoRedo;

[Tool]
public partial class PolygonPoint3D : Sprite3D
{
    public static readonly PackedScene Scene = GD.Load<PackedScene>("res://apps/drag_drop/scenes/polygon_point_3d.tscn");

    private static class NodePaths
    {
        public static readonly NodePath Draggable = "Draggable";
        public static readonly NodePath Collider = "Draggable/CollisionShape";
        public static readonly NodePath LineRenderer = "LineRenderer";
        public static readonly NodePath InHandle = "InHandle";
        public static readonly NodePath OutHandle = "OutHandle";
        public static class Property
        {
            public static readonly NodePath Modulate = "modulate";
        }
    }

    [Signal]
    public delegate void PositionChangedEventHandler(Vector2 position);

    [Signal]
    public delegate void InHandleChangedEventHandler(Vector2 position);

    [Signal]
    public delegate void OutHandleChangedEventHandler(Vector2 position);

    private float _inputMargin = 0.05f;
    private Draggable3D.Mode _dropMode = Draggable3D.Mode.Anywhere;
    private Array<NodePath> _dropAreas = [];
    private float _size = 0.5f;
    private Color _inactiveModulate = Colors.White;
    private Color _activeModulate = Colors.White;
    private Color _handleInactiveModulate = new(Colors.White * 0.6f, 1);
    private Color _handleActiveModulate = new(Colors.White * 0.6f, 1);

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
    public Color ActiveModulate
    {
        get => _activeModulate;
        set
        {
            _activeModulate = value;
            Update();
        }
    }

    [Export]
    public Color HandleInactiveModulate
    {
        get => _handleInactiveModulate;
        set
        {
            _handleInactiveModulate = value;
            Update();
        }
    }

    [Export]
    public Color HandleActiveModulate
    {
        get => _handleActiveModulate;
        set
        {
            _handleActiveModulate = value;
            Update();
        }
    }

    public History? History;
    public Func<Vector2, Vector2>? CustomSnapMethod;

    public bool EnableInHandle = false;
    public bool EnableOutHandle = false;

    private Viewport? Viewport;
    private Camera3D? Camera;
    private Draggable3D? Draggable;
    private CollisionShape3D? Collider;
    private PolygonPoint3D? InHandle;
    private PolygonPoint3D? OutHandle;
    private PolygonPoint3D?[] Handles => [InHandle, OutHandle];
    private Node2D? LineRenderer;

    public override void _EnterTree()
    {
        Viewport = GetViewport();
        Camera = Viewport?.GetCamera3D();
    }

    public override void _Ready()
    {
        Draggable = GetNode<Draggable3D>(NodePaths.Draggable);
        Collider = GetNode<CollisionShape3D>(NodePaths.Collider);
        InHandle = GetNodeOrNull<PolygonPoint3D>(NodePaths.InHandle);
        OutHandle = GetNodeOrNull<PolygonPoint3D>(NodePaths.OutHandle);
        LineRenderer = GetNodeOrNull<Node2D>(NodePaths.LineRenderer);
        Modulate = InactiveModulate;
        if (Draggable is not null)
        {
            Draggable.DragStarted += OnDragStarted;
            Draggable.DragFinished += OnDragFinished;
            Draggable.PositionChanged += OnPositionChanged;
        }
        if (InHandle is not null && OutHandle is not null)
        {
            InHandle.PositionChanged += OnInHandleChanged;
            OutHandle.PositionChanged += OnOutHandleChanged;
        }
        if (LineRenderer is not null)
            LineRenderer.Draw += OnDraw;
        Update();
    }

    private void OnDraw()
    {
        if (LineRenderer is null || Camera is null || InHandle is null || OutHandle is null) return;
        var screenPos = Camera.UnprojectPosition(GlobalPosition);
        if (EnableInHandle && !Camera.IsPositionBehind(InHandle.GlobalPosition))
        {
            var isDragging = InHandle.Draggable!.Dragging || Draggable!.Dragging;
            var screenInHandle = Camera.UnprojectPosition(InHandle.GlobalPosition);
            LineRenderer.DrawDashedLine(screenPos, screenInHandle, isDragging ? HandleActiveModulate : HandleInactiveModulate, 3, 10);
        }
        if (EnableOutHandle && !Camera.IsPositionBehind(OutHandle.GlobalPosition))
        {
            var isDragging = OutHandle.Draggable!.Dragging || Draggable!.Dragging;
            var screenOutHandle = Camera.UnprojectPosition(OutHandle.GlobalPosition);
            LineRenderer.DrawDashedLine(screenPos, screenOutHandle, isDragging ? HandleActiveModulate : HandleInactiveModulate, 3, 10);
        }
    }

    public override void _Process(double delta)
    {
        LineRenderer?.QueueRedraw();
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
        if (InHandle is not null) InHandle.Visible = EnableInHandle;
        if (OutHandle is not null) OutHandle.Visible = EnableOutHandle;
        if (LineRenderer is not null) LineRenderer.Visible = EnableInHandle || EnableOutHandle;
        Handles.WhereNotNull().ForEach(UpdateHandle);
        SetProcess(EnableInHandle || EnableOutHandle);
    }

    private void UpdateHandle(PolygonPoint3D handle)
    {
        handle.Size = Size * 0.8f;
        handle.DropMode = DropMode;
        handle.DropAreas = DropAreas;
        handle.InputMargin = InputMargin;
        handle.InactiveModulate = HandleInactiveModulate;
        handle.ActiveModulate = HandleActiveModulate;
        handle.History = History;
    }

    private float GetPixelSize()
    {
        if (Texture is null) return 0.01f;
        var textureSize = Texture.GetSize();
        return Size / ((textureSize.X + textureSize.Y) / 2);
    }

    private void Move(Vector3 position)
    {
        GlobalPosition = position;
        EmitSignal(SignalName.PositionChanged, position.ToVector2());
    }

    private void OnDragStarted() => CreateTween().TweenProperty(this, NodePaths.Property.Modulate, ActiveModulate, 0.125);
    private void OnDragFinished() => CreateTween().TweenProperty(this, NodePaths.Property.Modulate, InactiveModulate, 0.125);
    private void OnPositionChanged(Vector3 newPosition, Vector3 oldPosition)
    {
        if (History is not null) History.Add(new MovePoint(Move, newPosition, oldPosition));
        else Move(newPosition);
    }

    public Vector2 ToLocal(Vector2 position) => ToLocal(position.ToVector3()).ToVector2();
    public Vector2 ToGlobal(Vector2 position) => ToGlobal(position.ToVector3()).ToVector2();

    private void OnInHandleChanged(Vector2 position)
    {
        EmitSignal(SignalName.InHandleChanged, [ToLocal(position)]);
    }

    private void OnOutHandleChanged(Vector2 position)
    {
        EmitSignal(SignalName.OutHandleChanged, [ToLocal(position)]);
    }

    public void SetInHandlePosition(Vector2 position)
    {
        InHandle?.Draggable?.ForceSetGlobalPosition(ToGlobal(position).ToVector3());
    }

    public void SetOutHandlePosition(Vector2 position)
    {
        OutHandle?.Draggable?.ForceSetGlobalPosition(ToGlobal(position).ToVector3());
    }

    public static PolygonPoint3D Instantiate(int index)
    {
        var node = Scene.Instantiate<PolygonPoint3D>();
        node.Index = index;
        return node;
    }
}