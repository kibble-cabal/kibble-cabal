using Godot;

public partial class BuildModeState : Node
{
    public History History = new();

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed(History.InputAction.Undo))
            History.Undo();
        if (@event.IsActionPressed(History.InputAction.Redo))
            History.Redo();
    }

    public static BuildModeState? GetState() => GameModeSubSystem.State as BuildModeState;
    public static History? GetHistory() => GetState()?.History;

    // TODO UI
}