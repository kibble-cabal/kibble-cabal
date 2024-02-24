using Godot;
using UndoRedo;

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
            if (Start is null || End is null || Building is null) return;
            var start = (Start ?? Vector3.Inf).ToVector2();
            var end = (End ?? Vector3.Inf).ToVector2();
            var startHandle = (StartHandle ?? Vector3.Zero).ToVector2();
            var endHandle = (EndHandle ?? Vector3.Zero).ToVector2();
            History?.Add(new UndoRedo.Add<Wall>(Building, new Wall(start, end, startHandle, endHandle)));
            UIRoot?.Pop();
        }

        public void OnCancelButtonPressed() => UIRoot?.Pop();
    }
}
