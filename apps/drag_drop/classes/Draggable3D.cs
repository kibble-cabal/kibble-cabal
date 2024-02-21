using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class Draggable3D : Area3D
{
    public static readonly StringName ClickAction = "click";
    public enum Mode
    {
        Anywhere,
        AnyDropArea,
        SomeDropAreas
    }

    [Signal]
    public delegate void DragStartedEventHandler();

    [Signal]
    public delegate void DragFinishedEventHandler();

    [Signal]
    public delegate void AttemptedDropEventHandler(Vector3 position);

    [Signal]
    public delegate void DroppedEventHandler(Droppable3D? dropArea, Vector3 position);

    [Signal]
    public delegate void PositionChangedEventHandler(Vector3 newPosition, Vector3 oldPosition);

    private Mode _mode = Mode.AnyDropArea;
    private bool _rayCastSimple = true;
    private bool _dragging = false;

    [Export]
    public Node3D? ParentNode;

    [Export]
    public Node3D? GhostNode;

    [ExportGroup("Drop", "Drop")]

    [Export]
    public Mode DropMode
    {
        get => _mode;
        set => this.Set(ref _mode, value);
    }

    public Array<NodePath> DropAreas = [];

    [ExportGroup("Snapping", "Snap")]
    [Export]
    public bool SnapEnabled = true;

    [ExportGroup("AxisLock", "AxisLock")]
    [Export]
    public bool AxisLockX = false;
    [Export]
    public bool AxisLockY = false;
    [Export]
    public bool AxisLockZ = false;

    [ExportGroup("RayCasting")]
    [Export]
    public bool RayCastSimple
    {
        get => _rayCastSimple;
        set => this.Set(ref _rayCastSimple, value);
    }

    [Export]
    public bool RayShowDebug = false;

    public float RayLength = 100;

    private Viewport? Viewport;
    private Camera3D? Camera;

    public bool Dragging
    {
        get => _dragging;
        private set
        {
            if (!_dragging && value) EmitSignal(SignalName.DragStarted);
            if (_dragging && !value) EmitSignal(SignalName.DragFinished);
            _dragging = value;
        }
    }

    public Vector3 StartPosition;
    public Func<Vector3, Vector3>? CustomSnapMethod;
    public PhysicsRayQueryParameters3D Query = new();
    public Dictionary QueryResult = [];

    public Draggable3D()
    {
        CollisionLayer = 1 << 9; // ui_drag
        CollisionMask = 1 << 10; // ui_drop
        if (!Engine.IsEditorHint())
        {
            InputEvent += (_, @event, _, _, _) => OnInputEvent(@event);
            DragStarted += OnDragStarted;
            DragFinished += OnDragFinished;
        }
    }

    public override void _EnterTree()
    {
        Viewport = GetViewport();
        Camera = Viewport?.GetCamera3D();
    }

    public override void _Ready()
    {
        StartPosition = ParentNode?.Position ?? Vector3.Zero;
        if (!Engine.IsEditorHint())
        {
            Query.CollideWithAreas = true;
            Query.CollisionMask = 1 << 11; // ui_physics_ray
        }
    }

    public override Array<Dictionary> _GetPropertyList()
    {
        var dropAreasProperty = new ExportedProperty(
            name: nameof(DropAreas),
            type: Variant.Type.Array,
            hint: PropertyHint.ArrayType,
            hintString: nameof(NodePath),
            usage: PropertyUsageFlags.NoEditor
        );
        var rayLengthProperty = new ExportedProperty(
            name: nameof(RayLength),
            type: Variant.Type.Float,
            usage: PropertyUsageFlags.NoEditor
        );

        if (DropMode == Mode.SomeDropAreas)
            dropAreasProperty.Usage = PropertyUsageFlags.Default;

        if (!RayCastSimple)
            rayLengthProperty.Usage = PropertyUsageFlags.Default;

        return [
            dropAreasProperty,
            rayLengthProperty
        ];
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!Dragging || Engine.IsEditorHint()) return;
        Viewport?.SetInputAsHandled();
        if (@event is InputEventMouseMotion && QueryResult.ContainsKey("position"))
            SetGlobalPosition(QueryResult["position"].As<Vector3>());
        if (@event.IsActionReleased(ClickAction))
            Dragging = false;
    }

    public override void _Process(double delta)
    {
        if (RayShowDebug && Dragging)
            DebugDraw3D.DrawArrow(Query.From + new Vector3(0.1f, 0, 0), Query.To, Colors.Red, 0.1f);
    }

    public override void _PhysicsProcess(double delta)
    {
        // Lerp back to start position when drag failed
        if (!Dragging && IsNodeReady() && !Engine.IsEditorHint() && ParentNode is not null)
            ParentNode.Position = ParentNode.Position.Lerp(StartPosition, 0.1f);

        if (!Dragging || Engine.IsEditorHint() || Camera is null) return;

        // Do raycasting
        var mousePosition = Viewport?.GetMousePosition() ?? Vector2.Zero;
        Query.From = Camera.GlobalPosition;
        if (RayCastSimple)
        {
            Query.To = Camera.ProjectToFloor(mousePosition);
            QueryResult["position"] = Query.To;
        }
        else
        {
            Query.To = Camera.ProjectPosition(mousePosition, RayLength);
            QueryResult = GetWorld3D().DirectSpaceState.IntersectRay(Query);
        }
    }

    private void OnInputEvent(InputEvent @event)
    {
        if (Engine.IsEditorHint()) return;
        if (@event.IsActionPressed(ClickAction))
        {
            Viewport?.SetInputAsHandled();
            Dragging = true;
        }
        if (@event.IsActionReleased(ClickAction))
        {
            Viewport?.SetInputAsHandled();
            Dragging = false;
        }
    }

    private void OnDragStarted()
    {
        StartPosition = ParentNode?.GlobalPosition ?? Vector3.Zero;
    }

    private void OnDragFinished()
    {
        if (ParentNode is null) return;
        if (CanDrop())
        {
            var oldPosition = StartPosition;
            StartPosition = ParentNode.GlobalPosition;
            var dropArea = GetDropArea();
            dropArea?.Drop(this, ParentNode.GlobalPosition);
            EmitSignal(SignalName.Dropped, [Variant.From(dropArea), ParentNode.GlobalPosition]);
            EmitSignal(SignalName.PositionChanged, [ParentNode.GlobalPosition, oldPosition]);
        }
        else EmitSignal(SignalName.AttemptedDrop, [ParentNode.GlobalPosition]);
    }

    private void SetGlobalPosition(Vector3 position)
    {
        if (ParentNode is null) return;
        ParentNode.GlobalPosition = Locked(position);
        Snap();
    }

    private void Snap()
    {
        if (!SnapEnabled || ParentNode is null) return;
        foreach (var area in GetDropAreas().Reverse())
            ParentNode.GlobalPosition = Locked(area.Snap(ParentNode.GlobalPosition));
        if (CustomSnapMethod is Func<Vector3, Vector3> method)
            ParentNode.GlobalPosition = method(ParentNode.GlobalPosition);
    }

    private Vector3 Locked(Vector3 globalPos)
    {
        if (ParentNode is null) return globalPos;
        return new Vector3(
            AxisLockX ? ParentNode.GlobalPosition.X : globalPos.X,
            AxisLockY ? ParentNode.GlobalPosition.Y : globalPos.Y,
            AxisLockZ ? ParentNode.GlobalPosition.Z : globalPos.Z
        );
    }

    private Droppable3D? GetDropArea()
    {
        if (DropMode != Mode.Anywhere) return GetDropAreas().FirstOrDefault();
        return null;
    }

    private IEnumerable<Droppable3D> GetDropAreas()
    {
        var areas = GetOverlappingAreas()
            .Where(area => area is Droppable3D)
            .Select(area => area as Droppable3D)
            .WhereNotNull();
        if (DropMode == Mode.SomeDropAreas)
            areas = areas.Intersect(DropAreas.Select(GetNode<Droppable3D>));
        return areas.OrderByDescending(area => area.Priority);
    }

    private bool CanDrop() => DropMode switch
    {
        Mode.Anywhere => true,
        Mode.AnyDropArea or Mode.SomeDropAreas => GetDropAreas().Any(),
        _ => false
    };
}
