using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Godot;
using Godot.Collections;

using static Godot.Mathf;

[Tool]
[GlobalClass]
public partial class CircleContainerV2 : Container
{
    public enum Align
    {
        Inside,
        Center,
    }

    private Array<Control> _unsortedNodes = [];
    private float _extraMargin = 0;
    private Align _alignChildren = Align.Center;
    private float _degreeMin = 0;
    private float _degreeMax = 360;

    [Export]
    public Array<Control> UnsortedNodes
    {
        get => _unsortedNodes;
        set
        {
            _unsortedNodes = value;
            QueueSort();
        }
    }

    [Export]
    public float ExtraMargin
    {
        get => _extraMargin;
        set
        {
            _extraMargin = value;
            QueueSort();
        }
    }

    [Export]
    public Align AlignChildren
    {
        get => _alignChildren;
        set
        {
            _alignChildren = value;
            QueueSort();
        }
    }

    [Export(PropertyHint.Range, "0,360")]
    public float DegreeMin
    {
        get => _degreeMin;
        set
        {
            _degreeMin = value;
            QueueSort();
        }
    }

    [Export(PropertyHint.Range, "0,360")]
    public float DegreeMax
    {
        get => _degreeMax;
        set
        {
            _degreeMax = value;
            QueueSort();
        }
    }

    public Vector2 Center => Size / 2;
    public float Radius => Min(Size.X, Size.Y) / 2 - ExtraMargin;
    public int NumChildren
    {
        get
        {
            if (IsInsideTree()) return GetChildCount() - UnsortedNodes.Count;
            return 0;
        }
    }
    public float DegreeIncrement => DegreeRange / Max(NumChildren, 1);
    public float DegreeRange => DegreeMax - DegreeMin;

    public override void _Notification(int what)
    {
        if (what.IsNotification(NotificationDraw, NotificationVisibilityChanged, NotificationResized, NotificationSortChildren, NotificationReady))
            Sort();
    }

    public override Vector2 _GetMinimumSize()
    {
        var children = GetControlledChildren();
        var width = children.Select(child => child.Size.X).OrderDescending().Take(2).Sum();
        var height = children.Select(child => child.Size.Y).OrderDescending().Take(2).Sum();
        var side = Max(width, height);
        return side.ToVector2();
    }

    protected IEnumerable<Control> GetControlledChildren() => GetChildren()
        .Where(child => child is Control)
        .Select(child => child as Control)
        .Except(UnsortedNodes)
        .WhereNotNull();

    private float Theta(int index) => 360f.ToRad() - (360 - DegreeMin).ToRad() + DegreeIncrement.ToRad() * index - 90f.ToRad();

    private Vector2 FindPositionInside(Control child, float theta)
    {
        float f = Floor(2 * theta / Pi);
        float th = Pow(-1, f) * (theta - Floor((f + 1) / 2) * Pi);
        float b = child.Size.X * Cos(th) + child.Size.Y * Sin(th);
        float d = (Pow(b * b + 4 * Radius * Radius - Pow(child.Size.X, 2) - Pow(child.Size.Y, 2), 0.5f) - b) / 2;
        var pos = new Vector2(d * Cos(theta), d * Sin(theta));
        return Center + pos - child.Size / 2;
    }

    private Vector2 FindPositionCenter(Control child, float theta) => Center + Vector2.FromAngle(theta) * Radius - child.Size / 2;

    private Vector2 FindPosition(Control child, float theta) => AlignChildren switch
    {
        Align.Inside => FindPositionInside(child, theta),
        Align.Center => FindPositionCenter(child, theta),
        _ => throw new UnreachableException()
    };

    public void Sort()
    {
        // Sort controlled children
        var children = GetControlledChildren();
        children.ForEach((child, i) =>
         {
             var size = child.GetMinimumSize();
             float theta = Theta(i);
             if (size.X / size.Y > 1.5) // Make horizontal shapes a little closer together vertically
             {
                 float effect = theta.Wrap(0, Pi / 2).Sin();
                 float deg = theta.ToDeg().Wrap(0, 180);
                 if ((deg - 90).Abs() < 5 || (deg - 180).Abs() < 15) { }
                 else if (deg < 90) theta -= 5f.ToRad();
                 else theta += 5f.ToRad();
             }
             Vector2 pos = FindPosition(child, theta);
             FitChildInRect(child, new Rect2(pos, size));
         });

        // Sort uncontrolled children
        UnsortedNodes.WhereNotNull().ForEach(child => FitChildInRect(child, FindUnsortedRect(child)));
    }

    private Rect2 FindUnsortedRect(Control node)
    {
        var centeredPosition = (Size - node.Size) / 2;
        var endPosition = Size - node.Size;
        var (width, x) = node.SizeFlagsHorizontal switch
        {
            SizeFlags.Fill or SizeFlags.ExpandFill => (Size.X, 0),
            SizeFlags.Expand => (node.Size.X, centeredPosition.X),
            SizeFlags.ShrinkBegin => (node.Size.X, 0),
            SizeFlags.ShrinkCenter => (node.Size.X, centeredPosition.X),
            SizeFlags.ShrinkEnd => (node.Size.X, endPosition.X),
            _ => throw new UnreachableException()
        };
        var (height, y) = node.SizeFlagsVertical switch
        {
            SizeFlags.Fill or SizeFlags.ExpandFill => (Size.Y, 0),
            SizeFlags.Expand => (node.Size.Y, centeredPosition.Y),
            SizeFlags.ShrinkBegin => (node.Size.Y, 0),
            SizeFlags.ShrinkCenter => (node.Size.Y, centeredPosition.Y),
            SizeFlags.ShrinkEnd => (node.Size.Y, endPosition.Y),
            _ => throw new UnreachableException()
        };
        return new Rect2(x, y, width, height);
    }
}