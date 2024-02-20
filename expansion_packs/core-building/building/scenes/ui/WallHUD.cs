using Godot;
using System;

namespace KibbleCabal.Core.Building.UI
{
    public partial class WallHUD : Control3DV2
    {
        public static readonly PackedScene Scene = GD.Load<PackedScene>("res://expansion_packs/core-building/building/scenes/ui/wall_hud.tscn");

        [Export]
        public RBuilding? Building;

        [Export]
        public int Index = -1;

        private void OnMoveButtonPressed() => Building?.EmitSignal(RBuilding.SignalName.MoveWallRequested, [Index]);
        private void OnDestroyButtonPressed() => Building?.EmitSignal(RBuilding.SignalName.DestroyWallRequested, [Index]);

        public void Update()
        {
            var center = Building?.Get<Wall>(Index)?.GetCentroid() ?? Vector2.Zero;
            LocalPosition = center.ToVector3(Vector3.Axis.Y, 1);
        }

        public static WallHUD Instantiate(RBuilding building, int index)
        {
            var scene = Scene.Instantiate<WallHUD>();
            scene.Building = building;
            scene.Index = index;
            scene.Update();
            return scene;
        }
    }
}