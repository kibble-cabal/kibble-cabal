using Godot;

namespace KibbleCabal.Core.Building.UI
{
    public partial class EditingBuildingUI : VBoxContainer
    {
        private static class NodePaths
        {
            public static NodePath Spawner = "Spawner";
        }

        private static readonly PackedScene AddWallUI = GD.Load<PackedScene>("res://expansion_packs/core-building/building/scenes/ui/add_wall_ui.tscn");
        private static readonly Vector2[] SquareRoomPoints = [
            Vector2.Zero,
            new Vector2(0, 2),
            new Vector2(2, 2),
            new Vector2(2, 0),
            Vector2.Zero
        ];

        [Export]
        public RBuilding? Building;

        private Node3D? World;
        private UIStack? UIRoot => this.GetGameModeUIRoot();
        private static History? History => BuildModeState.GetHistory();

        public override void _Ready()
        {
            World = GetNode<Node3D>(NodePaths.Spawner);
            if (Building is not null)
            {
                Building.DestroyWallRequested += OnDestroyWallRequested;
                Building.DestroyFloorRequested += OnDestroyFloorRequested;
                Building.MoveWallRequested += OnMoveWallRequested;
                Building.MoveFloorRequested += OnMoveFloorRequested;
                History?.OnAfterUndo("Add Building", OnUndoAddBuilding);
            }
            Respawn();
        }

        public override void _EnterTree() => Respawn();

        private void Respawn()
        {
            if (Building is null || World is null) return;

            World.QueueFreeChildren();

            for (int i = 0; i < Building.Walls.Count; i++)
            {
                new WallUISpawner(Building, i).Spawn(World);
                new WallPolygonUISpawner(Building, i).Spawn(World);
            }

            for (int i = 0; i < Building.Floors.Count; i++)
                new FloorUISpawner(Building, i).Spawn(World);
        }

        private void InitiateMove(int[] walls, int[] floors)
        {
            if (UIRoot is null || Building is null) return;
            var scene = MoveUI.Instantiate(Building, walls, floors);
            UIRoot.Push(scene);
        }

        private void OnDestroyWallRequested(int index)
        {
            var wall = Building?.Get<Wall>(index);
            if (Building is null || wall is null) return;
            History?.Add(
                "Destroy Wall",
                () => Building.Remove<Wall>(index),
                () => Building.Add<Wall>(wall)
            );
        }

        private void OnDestroyFloorRequested(int index)
        {
            var floor = Building?.Get<Floor>(index);
            if (Building is null || floor is null) return;
            History?.Add(
                "Destroy Floor",
                () => Building.Remove<Floor>(index),
                () => Building.Add<Floor>(floor)
            );
        }

        private void OnMoveWallRequested(int index)
        {
            if (Building is null) return;
            UIRoot?.Push(MoveUI.Instantiate(Building, walls: [index], floors: []));
        }

        private void OnMoveFloorRequested(int index)
        {
            if (Building is null) return;
            UIRoot?.Push(MoveUI.Instantiate(Building, floors: [index], walls: []));
        }

        private void OnUndoAddBuilding(HistoryItem item) => UIRoot?.Pop();

        private void OnAddWallButtonPressed()
        {
            if (UIRoot is null) return;
            var scene = AddWallUI.Instantiate<AddWallUI>();
            scene.Building = Building;
            UIRoot.Push(scene);
        }

        private void OnAddFloorButtonPressed()
        {
            // TODO
        }

        private void OnCreateSquareRoomButtonPressed()
        {
            int[] walls = Building?.Add<Wall>(SquareRoomPoints) ?? [];
            int[] floors = Building is null ? [] : [Building.Add<Floor>(SquareRoomPoints)];
            InitiateMove(walls, floors);
        }

        private void OnDoneButtonPressed() => UIRoot?.Pop();
    }
}