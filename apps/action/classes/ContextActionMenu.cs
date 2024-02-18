using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class ContextActionMenu : CircleContainerV2
{
    [Signal]
    public delegate void OpeningEventHandler();

    [Signal]
    public delegate void OpenedEventHandler();

    [Signal]
    public delegate void ClosingEventHandler();

    [Signal]
    public delegate void ClosedEventHandler();

    public StringName MenuIdentifier = "";
    public Array<RContextAction> AdditionalActions = [];
    public bool CloseOnSelect = true;

    private Godot.Collections.Dictionary<RContextAction, Button> Nodes = [];

    public override void _EnterTree() => Visible = false;

    public IEnumerable<RContextAction> GetAllActions() => [.. AdditionalActions, .. ContextActionDB.FindByMenu(MenuIdentifier)];

    public void Open<Ctx>(Ctx? ctx) where Ctx : class
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

    private void UpdateItems<Ctx>(Ctx? ctx) where Ctx : class
    {
        var actions = GetAllActions();

        // Remove outdated actions
        Nodes.Keys
            .Except(actions)
            .Where(action => Nodes[action].CanQueueFree())
            .ForEach(action => Nodes[action].QueueFree());

        // Add missing actions
        foreach (var action in actions)
        {
            Nodes[action] = action.Render(ctx);
            Nodes[action].TryConnect(BaseButton.SignalName.Pressed, Callable.From(OnItemPressed));
            AddChild(Nodes[action]);
        }

        // Update all actions
        Nodes.Keys.ForEach(action => action.Update(Nodes[action], ctx));
    }

    private void OnItemPressed()
    {
        if (CloseOnSelect) Close();
    }
}