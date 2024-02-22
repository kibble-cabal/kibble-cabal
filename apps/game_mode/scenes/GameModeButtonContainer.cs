using System.Linq;
using Godot;
using Godot.Collections;

namespace KibbleCabal.Apps.GameMode
{
    [Tool]
    public partial class GameModeButtonContainer : CircleContainer
    {
        private static readonly PackedScene ButtonScene = GD.Load<PackedScene>("res://apps/game_mode/scenes/game_mode_button.tscn");

        private Dictionary<RGameMode, GameModeButton> Nodes = [];
        private ColorRect? Background;

        public override void _Ready()
        {
            Background = GetNode<ColorRect>("ColorRect");
            Update();
        }

        private void UpdateFocus()
        {
            if (GameModeSubSystem.Current is RGameMode current && Nodes.TryGetValue(current, out var value))
                value.GrabFocus();
        }

        private void Update()
        {
            if (Engine.IsEditorHint()) return;

            // Add missing nodes
            GameModeDB.Resources.Where(mode => !Nodes.ContainsKey(mode)).ForEach(Render);

            // Remove outdated nodes
            Nodes.Keys.Except(GameModeDB.Resources).Intersect(Nodes.Keys).ForEach(Remove);

            // Sort nodes
            foreach (var gameMode in Nodes.Keys.OrderBy(mode => mode.UIMenuIndex))
                MoveChild(Nodes[gameMode], gameMode.UIMenuIndex);
        }

        private void Render(RGameMode gameMode)
        {
            var scene = ButtonScene.Instantiate<GameModeButton>();
            scene.GameMode = gameMode;
            Nodes[gameMode] = scene;
            AddChild(scene);
        }

        private void Remove(RGameMode gameMode)
        {
            Nodes[gameMode].QueueFree();
            Nodes.Remove(gameMode);
        }

        public override void Sort()
        {
            base.Sort();
            // Rotate buttons
            var children = GetControlledChildren();
            var numChildren = children.Count();
            children.ForEach((child, i) => child.RotationDegrees = i * 30f / numChildren - 30f / 2f);

            // Update background
            if (Background is not null)
            {
                Background.PivotOffset = Background.Size / 2;
                Background.RotationDegrees = -30;
            }
        }

    }
}