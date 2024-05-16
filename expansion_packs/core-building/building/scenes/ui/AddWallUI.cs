using Godot;
using UndoRedo;

namespace KibbleCabal.Core.Building.UI
{
    public partial class AddWallUI : VBoxContainer
    {
        private static readonly StringName ClickAction = "click";

        public RBuilding? Building;
        private Wall Wall = new();
        // private Vector3? Start;
        // private Vector3? StartHandle;
        // private Vector3? End;
        // private Vector3? EndHandle;
        private Viewport? Viewport;
        private Camera3D? Camera;
        private Node3D? World;
        private WallPolygonUISpawner? UISpawner;
        private UIStack? UIRoot => this.GetGameModeUIRoot();
        private static History? History => BuildModeState.GetHistory();

        public override void _Ready()
        {
            Viewport = GetViewport();
            Camera = Viewport?.GetCamera3D();
            World = GetNode<Node3D>("Spawner");

            var building = new RBuilding();
            building.Add(Wall);
            UISpawner = new WallPolygonUISpawner(building, 0);
            if (World is not null) UISpawner?.Spawn(World);
        }

        public override void _ExitTree()
        {
            UISpawner?.Despawn();
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventScreenDrag)
            {
                Viewport?.SetInputAsHandled();
                var position = Camera?.ProjectToFloor(GetGlobalMousePosition()).ToVector2() ?? Vector2.Inf;
                if (!Wall.End.IsFinite()) Wall.StartHandle = position;
                else Wall.EndHandle = position;
            }

            if (@event.IsActionPressed(ClickAction))
            {
                Viewport?.SetInputAsHandled();
                var position = Camera?.ProjectToFloor(GetGlobalMousePosition()).ToVector2() ?? Vector2.Inf;
                if (!Wall.Start.IsFinite()) Wall.Start = position;
                else Wall.End = position;
            }

            if (@event.IsActionReleased(ClickAction))
            {
                Viewport?.SetInputAsHandled();
                AddWall();
            }
        }

        private void AddWall()
        {
            if (!Wall.Start.IsFinite() || !Wall.End.IsFinite() || Building is null) return;
            // var start = (Start ?? Vector3.Inf).ToVector2();
            // var end = (End ?? Vector3.Inf).ToVector2();
            // var startHandle = (StartHandle ?? Vector3.Zero).ToVector2();
            // var endHandle = (EndHandle ?? Vector3.Zero).ToVector2();
            History?.Add(new UndoRedo.Add<Wall>(Building, Wall));
            UIRoot?.Pop();
        }

        public void OnCancelButtonPressed() => UIRoot?.Pop();
    }
}
