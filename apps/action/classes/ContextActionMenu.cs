using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

[GlobalClass]
public partial class ContextActionMenu : CircleContainer
{
    [Signal]
    public delegate void OpeningEventHandler();

    [Signal]
    public delegate void OpenedEventHandler();

    [Signal]
    public delegate void ClosingEventHandler();

    [Signal]
    public delegate void ClosedEventHandler();

    [Export]
    public StringName MenuIdentifier = "";

    public readonly List<IContextAction> AdditionalActions = [];

    [Export]
    public bool CloseOnSelect = true;

    [Export] 
    public bool CloseOnClickOutside = true;

    private readonly Dictionary<IContextAction, Button> Nodes = [];

    public override void _EnterTree() => Visible = false;

    public IEnumerable<IContextAction> GetAllActions() => [.. AdditionalActions, .. ContextActionDB.FindByMenu(MenuIdentifier)];

    public void Open<Ctx>(Ctx ctx)
    {
        EmitSignal(SignalName.Opening);
        UpdateItems(ctx);
        Show();
        EmitSignal(SignalName.Opened);
    }

    public void Close()
    {
        EmitSignal(SignalName.Closing);
        Hide();
        EmitSignal(SignalName.Closed);
    }

    private void UpdateItems<Ctx>(Ctx ctx)
    {
        var actions = GetAllActions();

        // Remove outdated actions
        Nodes.Values.QueueFreeAll();
        Nodes.Clear();

        // Add missing actions
        foreach (var action in actions)
        {
            Nodes[action] = (action as IContextAction<Ctx>)!.Render(ctx);
            Nodes[action].Pressed += OnItemPressed;
            AddChild(Nodes[action]);
        }
    }

    private void OnItemPressed()
    {
        if (CloseOnSelect) Close();
    }
    
    public override void _UnhandledInput(InputEvent @event)
    {
        if (Visible && CloseOnClickOutside && @event is InputEventScreenTouch && @event.IsPressed())
        {
            var menuRadius = Mathf.Max(Size.X, Size.Y) / 2;
            if (GetLocalMousePosition().DistanceTo(Size / 2) > menuRadius)
            {
                Close();
                GetViewport()?.SetInputAsHandled();
            }
        }
    }
}