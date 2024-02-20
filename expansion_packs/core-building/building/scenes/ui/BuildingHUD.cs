using Godot;

namespace KibbleCabal.Core.Building.UI
{
    public partial class BuildingHUD : Control3DV2
    {
        public static readonly PackedScene Scene = GD.Load<PackedScene>("res://expansion_packs/core-building/building/scenes/ui/building_hud.tscn");

        [Export]
        public RBuilding? Building;

        private void OnEditButtonPressed() => Building?.EmitSignal(RBuilding.SignalName.EditRequested);
        private void OnMoveButtonPressed() => Building?.EmitSignal(RBuilding.SignalName.MoveRequested);
        private void OnDestroyButtonPressed() => Building?.EmitSignal(RBuilding.SignalName.DestroyRequested);

        public void Update()
        {
            var center = Building?.GetCentroid() ?? Vector2.Zero;
            LocalPosition = center.ToVector3(Vector3.Axis.Y, 1);
        }

        public static BuildingHUD Instantiate(RBuilding building)
        {
            var scene = Scene.Instantiate<BuildingHUD>();
            scene.Building = building;
            scene.Update();
            return scene;
        }
    }
}
