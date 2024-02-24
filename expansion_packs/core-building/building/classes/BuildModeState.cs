using Godot;
using UndoRedo;

public partial class BuildModeState : Node
{
    public History History = new();
    public VBoxContainer HistoryNotificationContainer = new();

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed(History.InputAction.Undo))
            History.Undo();
        if (@event.IsActionPressed(History.InputAction.Redo))
            History.Redo();
    }

    public override void _Ready()
    {
        History.NotificationContainer = HistoryNotificationContainer;
        AddChild(HistoryNotificationContainer);
        HistoryNotificationContainer.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.BottomRight);
    }

    public static BuildModeState? GetState() => GameModeSubSystem.State as BuildModeState;
    public static History? GetHistory() => GetState()?.History;
}