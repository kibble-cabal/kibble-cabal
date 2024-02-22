using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KibbleCabal.Core.Building.UI
{
    public partial class BuildModeUI : Control
    {
        private static class NodePaths
        {
            public static readonly NodePath World = "World";
        }
        private static readonly PackedScene EditingBuildingUI = GD.Load<PackedScene>("res://expansion_packs/core-building/building/scenes/ui/editing_building_ui.tscn");

        private Node3D? World;
        private UIStack? UIRoot => this.GetGameModeUIRoot();
        private static History? History => BuildModeState.GetHistory();
        private static RLocationState? LocationState => LocationSubSystem.GetState();

        public override void _Ready()
        {
            World = GetNode<Node3D>(NodePaths.World);
            Respawn();
        }

        public override void _EnterTree()
        {
            if (LocationState is not null)
                LocationState.SpawnersChanged += Respawn;
            GetBuildings().ForEach(building =>
            {
                building.EditRequested += () => OnBuildingEditRequested(building);
                building.DestroyRequested += () => OnBuildingDestroyRequested(building);
                building.MoveRequested += () => OnBuildingMoveRequested(building);
            });
            Respawn();
        }

        public override void _ExitTree()
        {
            if (LocationState is not null)
                LocationState.SpawnersChanged -= Respawn;
            GetBuildings().ForEach(building =>
            {
                building.DisconnectAllFromTarget(RBuilding.SignalName.EditRequested, this);
                building.DisconnectAllFromTarget(RBuilding.SignalName.MoveRequested, this);
                building.DisconnectAllFromTarget(RBuilding.SignalName.DestroyRequested, this);
            });
            World?.QueueFreeChildren();
        }

        private void Respawn()
        {
            if (World is null) return;
            World.QueueFreeChildren();
            GetBuildings().ForEach(building => new BuildingUISpawner(building).Spawn(World));
        }

        private void OnCreateBuildingButtonPressed()
        {
            var building = new RBuilding();
            var scene = EditingBuildingUI.Instantiate<EditingBuildingUI>();
            scene.Building = building;
            UIRoot?.Push(scene);
        }

        private void OnBuildingEditRequested(RBuilding building)
        {
            var scene = EditingBuildingUI.Instantiate<EditingBuildingUI>();
            scene.Building = building;
            UIRoot?.Push(scene);
        }

        private void OnBuildingDestroyRequested(RBuilding building) => History?.Add(
            "Destroy Building",
            () => LocationState?.RemoveSpawnersFor(building),
            () => LocationState?.Add(new BuildingSpawner(building))
        );

        private void OnBuildingMoveRequested(RBuilding building) => UIRoot?.Push(MoveUI.Instantiate(
            building,
            [.. Enumerable.Range(0, building.Walls.Count)],
            [.. Enumerable.Range(0, building.Floors.Count)]
        ));

        private static IEnumerable<RBuilding> GetBuildings() => LocationState?.Get<BuildingSpawner>().Select(spawner => spawner.GetResource()).WhereNotNull() ?? [];
    }
}
