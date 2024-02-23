using System.Linq;
using Godot;
using Godot.Collections;

[Tool]
[GlobalClass]
public partial class Droppable3D : Area3D
{
    public enum SnapBehavior
    {
        None,
        CustomMesh,
        Points,
    }

    [Signal]
    public delegate void DroppedEventHandler(Draggable3D draggable, Vector3 dropPosition);

    [Export]
    public bool ReparentOnDrop = false;

    [Export]
    public bool DisableOnDrop = false;

    [ExportGroup("Snapping", "Snap")]

    private SnapBehavior _snapMode = SnapBehavior.None;

    [Export]
    public SnapBehavior SnapMode
    {
        get => _snapMode;
        set
        {
            _snapMode = value;
            NotifyPropertyListChanged();
        }
    }

    public float SnapThreshold = 0.1f;
    public bool SnapDebugEnabled = false;
    public Color SnapDebugColor = Colors.Red;
    public Mesh? SnapMesh;
    public Vector3[] SnapPoints = [];

    public Droppable3D()
    {
        CollisionLayer = (uint)Bit.Physics.UIDrop;
        CollisionMask = (uint)Bit.Physics.UIDrag;
        Monitoring = false;
        InputRayPickable = false;
    }

    public override Array<Dictionary> _GetPropertyList()
    {
        Array<Dictionary> properties = [];
        if (SnapMode != SnapBehavior.None)
        {
            properties.Add(new ExportedProperty(
                name: nameof(SnapThreshold),
                type: Variant.Type.Float
            ));
            properties.Add(new ExportedProperty(
                name: nameof(SnapDebugEnabled),
                type: Variant.Type.Bool
            ));
            properties.Add(new ExportedProperty(
                name: nameof(SnapDebugColor),
                type: Variant.Type.Color
            ));
            if (SnapMode == SnapBehavior.CustomMesh)
                properties.Add(new ExportedProperty(
                    name: nameof(SnapMesh),
                    type: Variant.Type.Object,
                    hint: PropertyHint.ResourceType,
                    hintString: nameof(Mesh)
                ));
            if (SnapMode == SnapBehavior.Points)
                properties.Add(new ExportedProperty(
                    name: nameof(SnapPoints),
                    type: Variant.Type.PackedVector3Array
                ));
        }
        return properties;
    }

    public override void _Process(double delta)
    {
        if (!SnapDebugEnabled) return;
        switch (SnapMode)
        {
            case SnapBehavior.Points:
                DebugDraw3D.DrawPoints([.. SnapPoints.Select(point => point * Transform.AffineInverse())], DebugDraw3D.PointType.TypeSquare, 0.1f, SnapDebugColor);
                break;
            case SnapBehavior.CustomMesh:
                SnapMesh?.DebugDrawMesh(Transform, 0.1f, SnapDebugColor);
                break;
        }
    }

    private Vector3 SnapToPoints(Vector3 globalPos) => SnapPoints.Closest(ToLocal(globalPos));
    private Vector3 SnapToMesh(Vector3 globalPos)
    {
        if (SnapMesh is null) return globalPos;
        var snapPos = SnapMesh.GetClosestPoint(ToLocal(globalPos));
        DebugDrawSnapFace(snapPos);
        return snapPos;
    }

    private void DebugDrawSnapFace(Vector3 localSnapPos)
    {
        if (!SnapDebugEnabled || SnapMesh is null) return;
        var face = SnapMesh.GetClosestFace(localSnapPos).Transformed(Transform.AffineInverse());
        DebugDraw3D.DrawPoints([.. face], DebugDraw3D.PointType.TypeSquare, 0.2f, SnapDebugColor * 1.1f);
    }

    public void Drop(Draggable3D draggable, Vector3 dropPosition)
    {
        if (ReparentOnDrop)
            draggable.ParentNode?.Reparent(this);
        if (DisableOnDrop)
            draggable.ProcessMode = ProcessModeEnum.Disabled;
        EmitSignal(SignalName.Dropped, [draggable, dropPosition]);
    }

    public Vector3 Snap(Vector3 globalPos)
    {
        var snapPos = SnapMode switch
        {
            SnapBehavior.Points => ToGlobal(SnapToPoints(globalPos)),
            SnapBehavior.CustomMesh => ToGlobal(SnapToMesh(globalPos)),
            _ => globalPos
        };
        if (snapPos.DistanceTo(globalPos).Abs() <= SnapThreshold && SnapThreshold >= 0)
            return snapPos;
        return globalPos;
    }
}