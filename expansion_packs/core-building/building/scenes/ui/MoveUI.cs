using Godot;

namespace KibbleCabal.Core.Building.UI
{
    public partial class MoveUI : VBoxContainer
    {
        public static readonly PackedScene Scene = GD.Load<PackedScene>("res://expansion_packs/core-building/building/scenes/ui/move_ui.tscn");

        [Export]
        public RBuilding? Building;

        [Export]
        public int[] Walls = [];

        [Export]
        public int[] Floors = [];

        [Export]
        public Vector3 StartPosition;

        private UIStack? UIRoot => this.GetGameModeUIRoot();
        private static History? History => BuildModeState.GetHistory();
        private static RLocationState? LocationState => LocationSubSystem.GetState();

        public override void _Ready() => this.StartPosition = GetViewport().GetCamera3D().ProjectToFloor(GetGlobalMousePosition());

        public static MoveUI Instantiate(RBuilding building, int[] walls, int[] floors)
        {
            var scene = Scene.Instantiate<MoveUI>();
            scene.Building = building;
            scene.Walls = walls;
            scene.Floors = floors;
            return scene;
        }

        private bool IsBuildingJustCreated()
        {
            if (Building is not null && LocationState is not null) return !LocationState.HasSpawnerFor(Building);
            return false;
        }

        private void TryCreateBuilding(Vector2 delta)
        {
            if (Building is null) return;
            Building.MoveBy<Wall>(Walls, delta);
            Building.MoveBy<Floor>(Floors, delta);
            History?.Add(
                "Add Building",
                () => LocationState?.Add(new BuildingSpawner(Building)),
                () => LocationState?.RemoveSpawnersFor(Building)
            );
        }

        public void OnCancelButtonPressed() => UIRoot?.Pop();

        public void OnCursorClicked(Vector3 position)
        {
            var delta = (position - StartPosition).ToVector2();
            if (IsBuildingJustCreated()) TryCreateBuilding(delta);
            else History?.Add(
                "Move",
                [() => Building?.MoveBy<Wall>(Walls, delta), () => Building?.MoveBy<Floor>(Floors, delta)],
                [() => Building?.MoveBy<Wall>(Walls, -delta), () => Building?.MoveBy<Floor>(Floors, -delta)]
            );
            UIRoot?.Pop();
        }
    }
}