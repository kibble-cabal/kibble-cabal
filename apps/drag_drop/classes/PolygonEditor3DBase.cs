using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

public abstract partial class PolygonEditor3DBase : Node3D
{
    protected static readonly StringName PointIndexMetaName = "PointIndex";
    protected static readonly PackedScene AddPointScene = GD.Load<PackedScene>("res://apps/drag_drop/scenes/add_point_button.tscn");
    protected static readonly PackedScene RemovePointScene = GD.Load<PackedScene>("res://apps/drag_drop/scenes/remove_point_button.tscn");

    [Signal]
    public delegate void PointChangedEventHandler(int index, Vector2 position);

    private bool _enableAddPoints = false;
    private bool _enableRemovePoints = false;
    private float _inputMargin = 0.05f;
    private Draggable3D.Mode _dropMode = Draggable3D.Mode.Anywhere;
    private Array<NodePath> _dropAreas = [];
    private float _size = 0.5f;
    private Color _inactiveModulate = Colors.White;
    private Color _draggingModulate = Colors.White;

    [ExportGroup("Behavior")]

    [Export]
    public bool EnableAddPoints
    {
        get => _enableAddPoints;
        set
        {
            _enableAddPoints = value;
            UpdateAddButtons();
        }
    }

    [Export]
    public bool EnableRemovePoints
    {
        get => _enableRemovePoints;
        set
        {
            _enableRemovePoints = value;
            UpdateRemoveButtons();
        }
    }

    [Export]
    public float InputMargin
    {
        get => _inputMargin;
        set
        {
            _inputMargin = value;
            UpdatePoints();
        }
    }

    [Export]
    public Draggable3D.Mode DropMode
    {
        get => _dropMode;
        set
        {
            _dropMode = value;
            UpdatePoints();
        }
    }

    [Export]
    public Array<NodePath> DropAreas
    {
        get => _dropAreas;
        set
        {
            _dropAreas = value;
            UpdatePoints();
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
            UpdatePoints();
        }
    }

    [Export]
    public Color InactiveModulate
    {
        get => _inactiveModulate;
        set
        {
            _inactiveModulate = value;
            UpdatePoints();
        }
    }

    [Export]
    public Color DraggingModulate
    {
        get => _draggingModulate;
        set
        {
            _draggingModulate = value;
            UpdatePoints();
        }
    }

    public History? History;
    public Func<Vector2, Vector2>? CustomSnapMethod;

    public List<PolygonPoint3D> Points { get; private set; } = [];
    public List<Button> AddButtons { get; private set; } = [];
    public List<Button> RemoveButtons { get; private set; } = [];

    public override void _Ready()
    {
        UpdatePoints();
    }

    protected abstract int _GetPointCount();
    protected abstract Vector2 _GetPointPosition(int index);
    protected abstract void _SetPointPosition(int index, Vector2 position);
    protected abstract void _AddPoint(int index, Vector2 position);
    protected abstract void _RemovePoint(int index);

    public Vector2 GetPointPosition(int index)
    {
        if (index >= 0 && index <= _GetPointCount())
            return _GetPointPosition(index);
        return Vector2.Inf;
    }

    public void SetPointPosition(int index, Vector2 position)
    {
        _SetPointPosition(index, position);
        EmitSignal(SignalName.PointChanged, position);
    }

    public Vector2 GetMidpoint(int index)
    {
        var pointCount = _GetPointCount();
        if (index >= 0 && index <= pointCount && pointCount >= 2)
        {
            var other = index > 0 ? GetPointPosition(index - 1) : GetPointPosition(pointCount - 1);
            return GetPointPosition(index).Midpoint(other);
        }
        return Vector2.Inf;
    }

    public void AddPoint(int index) => AddPoint(index, GetMidpoint(index));

    public void AddPoint(int index, Vector2 position)
    {
        var pointCount = _GetPointCount();
        if (index >= 0 && index <= pointCount && pointCount >= 2 && position.IsFinite())
        {
            _AddPoint(index, position);
            UpdatePoints();
        }
    }

    public void RemovePoint(int index)
    {
        if (index >= 0 && index < _GetPointCount())
        {
            _RemovePoint(index);
            UpdatePoints();
        }
    }

    private void UpdateNodeList<N>(List<N> list, int size, Func<int, N> make, Action<N, int> update) where N : Node
    {
        if (list.Count > size)
        {
            list.TakeLast(list.Count - size).ForEach(item => item.QueueFree());
            list.RemoveRange(size, list.Count - size);
        }
        for (int i = 0; i < size; i++)
        {
            if (list.Count > i) update(list[i], i);
            else
            {
                list.Add(make(i));
                AddChild(list[i]);
                update(list[i], i);
            }
        }
    }

    protected void UpdatePoints()
    {
        UpdateNodeList<PolygonPoint3D>(
            Points,
            _GetPointCount(),
            make: PolygonPoint3D.Instantiate,
            update: UpdatePoint
        );
        UpdateAddButtons();
        UpdateRemoveButtons();
    }

    protected void UpdateAddButtons() => UpdateNodeList<Button>(
        AddButtons,
        EnableAddPoints ? _GetPointCount() : 0,
        make: index =>
        {
            var button = AddPointScene.Instantiate<Button>();
            button.Pressed += () => OnAddButtonPressed(button);
            return button;
        },
        update: (button, index) =>
        {
            var position = GetMidpoint(index).ToVector3(Vector3.Axis.Y, Size);
            button.SetMeta(PointIndexMetaName, index);
            button.Set(Control3D.PropertyName.LocalPosition, position);
        }
    );

    protected void UpdateRemoveButtons() => UpdateNodeList<Button>(
        RemoveButtons,
        EnableRemovePoints ? _GetPointCount() : 0,
        make: index =>
        {
            var button = RemovePointScene.Instantiate<Button>();
            button.Pressed += () => OnRemoveButtonPressed(button);
            return button;
        },
        update: (button, index) =>
        {
            var position = GetPointPosition(index).ToVector3(Vector3.Axis.Y, Size);
            button.SetMeta(PointIndexMetaName, index);
            button.Set(Control3D.PropertyName.LocalPosition, position);
        }
    );

    protected virtual void UpdatePoint(PolygonPoint3D node, int index)
    {
        node.DisconnectAllFromTarget(PolygonPoint3D.SignalName.PositionChanged, this);
        node.Position = GetPointPosition(node.Index).ToVector3();
        node.History = History;
        node.Size = Size;
        node.InactiveModulate = InactiveModulate;
        node.DraggingModulate = DraggingModulate;
        node.DropAreas = DropAreas;
        node.DropMode = DropMode;
        node.InputMargin = InputMargin;
        node.CustomSnapMethod = CustomSnapMethod;
        node.Connect(PolygonPoint3D.SignalName.PositionChanged, Callable.From<Vector2>(position => SetPointPosition(index, position)));
    }

    protected virtual void OnAddButtonPressed(Button button)
    {
        var index = button.GetMeta(PointIndexMetaName, -1).As<int>();
        if (index != -1 && index < _GetPointCount())
        {
            if (History is not null) History.Add(
                "Add Point",
                () => AddPoint(index),
                () => RemovePoint(index)
            );
            else AddPoint(index);
        }
    }

    protected virtual void OnRemoveButtonPressed(Button button)
    {
        var index = button.GetMeta(PointIndexMetaName, -1).As<int>();
        if (index != -1 && index < _GetPointCount())
        {
            var position = GetPointPosition(index);
            if (History is not null) History.Add(
                "Remove Point",
                () => RemovePoint(index),
                () => AddPoint(index, position)
            );
            else RemovePoint(index);
        }
    }
}