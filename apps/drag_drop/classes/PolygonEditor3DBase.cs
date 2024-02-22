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

    [Signal]
    public delegate void InHandleChangedEventHandler(int index, Vector2 position);

    [Signal]
    public delegate void OutHandleChangedEventHandler(int index, Vector2 position);

    private bool _enableAddPoints = false;
    private bool _enableRemovePoints = false;
    private bool _enableHandles = false;
    private float _inputMargin = 0.05f;
    private Draggable3D.Mode _dropMode = Draggable3D.Mode.Anywhere;
    private Array<NodePath> _dropAreas = [];
    private float _size = 0.5f;
    private Color _inactiveModulate = Colors.White;
    private Color _activeModulate = Colors.White;
    private Color _handleInactiveModulate = new(Colors.White * 0.6f, 1);
    private Color _handleActiveModulate = new(Colors.White * 0.6f, 1);
    private History? _history;
    private Func<Vector2, Vector2>? _customSnapMethod;

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
    public bool EnableHandles
    {
        get => _enableHandles;
        set
        {
            _enableHandles = value;
            UpdatePoints();
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
    public Color ActiveModulate
    {
        get => _activeModulate;
        set
        {
            _activeModulate = value;
            UpdatePoints();
        }
    }

    [Export]
    public Color HandleInactiveModulate
    {
        get => _handleInactiveModulate;
        set
        {
            _handleInactiveModulate = value;
            UpdatePoints();
        }
    }

    [Export]
    public Color HandleActiveModulate
    {
        get => _handleActiveModulate;
        set
        {
            _handleActiveModulate = value;
            UpdatePoints();
        }
    }

    public History? History
    {
        get => _history;
        set
        {
            _history = value;
            UpdatePoints();
        }
    }
    public Func<Vector2, Vector2>? CustomSnapMethod
    {
        get => _customSnapMethod;
        set
        {
            _customSnapMethod = value;
            UpdatePoints();
        }
    }

    public List<PolygonPoint3D> Points { get; private set; } = [];
    public List<Button3D> AddButtons { get; private set; } = [];
    public List<Button3D> RemoveButtons { get; private set; } = [];

    public override void _Ready()
    {
        UpdatePoints();
    }

    protected abstract int _GetPointCount();
    protected abstract Vector2 _GetPointPosition(int index);
    protected abstract Vector2 _GetInHandlePosition(int index);
    protected abstract Vector2 _GetOutHandlePosition(int index);
    protected abstract void _SetPointPosition(int index, Vector2 position);
    protected abstract void _SetInHandlePosition(int index, Vector2 position);
    protected abstract void _SetOutHandlePosition(int index, Vector2 position);
    protected abstract void _AddPoint(int index, Vector2 position);
    protected abstract void _RemovePoint(int index);

    public bool HasPoint(int index) => index >= 0 && index <= _GetPointCount();

    public Vector2 GetPointPosition(int index)
    {
        if (HasPoint(index))
            return _GetPointPosition(index);
        return Vector2.Inf;
    }

    public Vector2 GetInHandlePosition(int index)
    {
        if (HasPoint(index))
            return _GetInHandlePosition(index);
        return Vector2.Inf;
    }

    public Vector2 GetOutHandlePosition(int index)
    {
        if (HasPoint(index))
            return _GetOutHandlePosition(index);
        return Vector2.Inf;
    }

    public void SetPointPosition(int index, Vector2 position)
    {
        if (HasPoint(index))
        {
            _SetPointPosition(index, position);
            EmitSignal(SignalName.PointChanged, [index, position]);
        }
    }

    public void SetInHandlePosition(int index, Vector2 position)
    {
        if (HasPoint(index))
        {
            _SetInHandlePosition(index, position);
            EmitSignal(SignalName.InHandleChanged, [index, position]);
        }
    }

    public void SetOutHandlePosition(int index, Vector2 position)
    {
        if (HasPoint(index))
        {
            _SetOutHandlePosition(index, position);
            EmitSignal(SignalName.OutHandleChanged, [index, position]);
        }
    }

    public Vector2 GetMidpoint(int index)
    {
        var pointCount = _GetPointCount();
        if (HasPoint(index) && pointCount >= 2)
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
        if (HasPoint(index) && pointCount >= 2 && position.IsFinite())
        {
            _AddPoint(index, position);
            UpdatePoints([index, index + 1]);
        }
    }

    public void RemovePoint(int index)
    {
        if (HasPoint(index))
        {
            _RemovePoint(index);
            UpdatePoints([index, index - 1]);
        }
    }

    private void UpdateNodeList<N>(List<N> list, int size, Func<int, N> make, Action<N, int> update, IEnumerable<int>? updateRange = null) where N : Node
    {
        if (list.Count > size)
        {
            list.TakeLast(list.Count - size).ForEach(item => item.QueueFree());
            list.RemoveRange(size, list.Count - size);
        }
        for (int i = 0; i < size; i++)
        {
            if (updateRange is not null && !updateRange.Contains(i)) continue;
            if (list.Count > i) update(list[i], i);
            else
            {
                list.Add(make(i));
                AddChild(list[i]);
                update(list[i], i);
            }
        }
    }

    public void UpdatePoints(IEnumerable<int>? updateRange = null)
    {
        UpdateNodeList<PolygonPoint3D>(
            Points,
            _GetPointCount(),
            make: index =>
            {
                var node = PolygonPoint3D.Instantiate(index);
                node.PositionChanged += position => SetPointPosition(index, position);
                node.InHandleChanged += position => SetInHandlePosition(index, position);
                node.OutHandleChanged += position => SetOutHandlePosition(index, position);
                return node;
            },
            update: UpdatePoint,
            updateRange: updateRange
        );
        UpdateAddButtons(updateRange);
        UpdateRemoveButtons(updateRange);
    }

    protected void UpdateAddButtons(IEnumerable<int>? updateRange = null) => UpdateNodeList<Button3D>(
        AddButtons,
        EnableAddPoints ? _GetPointCount() : 0,
        make: index =>
        {
            var button = AddPointScene.Instantiate<Button3D>();
            button.Pressed += () => OnAddButtonPressed(button);
            return button;
        },
        update: (button, index) =>
        {
            button.SetMeta(PointIndexMetaName, index);
            button.LocalPosition = GetMidpoint(index).ToVector3(Vector3.Axis.Y, Size);
        },
        updateRange
    );

    protected void UpdateRemoveButtons(IEnumerable<int>? updateRange = null) => UpdateNodeList<Button3D>(
        RemoveButtons,
        EnableRemovePoints ? _GetPointCount() : 0,
        make: index =>
        {
            var button = RemovePointScene.Instantiate<Button3D>();
            button.Pressed += () => OnRemoveButtonPressed(button);
            return button;
        },
        update: (button, index) =>
        {
            button.SetMeta(PointIndexMetaName, index);
            button.LocalPosition = GetPointPosition(index).ToVector3(Vector3.Axis.Y, Size);
        },
        updateRange
    );

    public virtual void UpdatePoint(PolygonPoint3D node, int index)
    {
        node.Position = GetPointPosition(node.Index).ToVector3();
        node.History = History;
        node.Size = Size;
        node.InactiveModulate = InactiveModulate;
        node.ActiveModulate = ActiveModulate;
        node.HandleActiveModulate = HandleActiveModulate;
        node.HandleInactiveModulate = HandleInactiveModulate;
        node.DropAreas = DropAreas;
        node.DropMode = DropMode;
        node.InputMargin = InputMargin;
        node.CustomSnapMethod = CustomSnapMethod;
        node.EnableInHandle = EnableHandles && index > 0;
        node.EnableOutHandle = EnableHandles && index < _GetPointCount() - 1;
        if (EnableHandles)
        {
            node.SetInHandlePosition(GetInHandlePosition(index));
            node.SetOutHandlePosition(GetOutHandlePosition(index));
        }
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