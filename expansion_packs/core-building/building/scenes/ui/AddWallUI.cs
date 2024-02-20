using Godot;

namespace KibbleCabal.Core.Building.UI
{
    public partial class AddWallUI : VBoxContainer
    {
        private static readonly StringName ClickAction = "click";

        public RBuilding? Building;

        private Vector3? Start;
        private Vector3? StartHandle;
        private Vector3? End;
        private Vector3? EndHandle;
        private Viewport? Viewport;
        private Camera3D? Camera;
        private UIStack? UIRoot => this.GetGameModeUIRoot();
        private static History? History => BuildModeState.GetHistory();

        public override void _Ready()
        {
            Viewport = GetViewport();
            Camera = Viewport?.GetCamera3D();
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventScreenDrag)
            {
                Viewport?.SetInputAsHandled();
                var position = Camera?.ProjectToFloor(GetGlobalMousePosition());
                if (End is null) StartHandle = position;
                else EndHandle = position;
            }

            if (@event.IsActionPressed(ClickAction))
            {
                Viewport?.SetInputAsHandled();
                var position = Camera?.ProjectToFloor(GetGlobalMousePosition());
                if (Start is null) Start = position;
                else End = position;
            }

            if (@event.IsActionReleased(ClickAction))
            {
                Viewport?.SetInputAsHandled();
                AddWall();
            }
        }

        private void AddWall()
        {
            if (Start is null || End is null) return;
            Vector3 start = Start ?? Vector3.Inf,
                end = End ?? Vector3.Inf,
                startHandle = StartHandle ?? Vector3.Zero,
                endHandle = EndHandle ?? Vector3.Zero;
            var index = Building?.Walls.Count ?? -1;
            History?.Add(
                "Add Wall",
                () => Building?.Add<Wall>(start.ToVector2(), end.ToVector2(), startHandle.ToVector2(), endHandle.ToVector2()),
                () => Building?.Remove<Wall>(index)
            );
            UIRoot?.Pop();
        }

        public void OnCancelButtonPressed() => UIRoot?.Pop();
    }
}
