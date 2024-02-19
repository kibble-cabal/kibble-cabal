using Godot;

namespace KibbleCabal.Apps.Settings
{
    public partial class SettingScene : HBoxContainer
    {
        private Node? UI;
        public RSettingDefinition? Setting;

        public override void _Ready()
        {
            Render();
            Setting?.TryConnectChanged(Callable.From(Render));
        }

        public void Render()
        {
            GetNode<Label>("%DisplayNameLabel").Text = Setting?.DisplayName;
            GetNode<Label>("%DisplayDescriptionLabel").Text = Setting?.DisplayDescription;
            UI?.QueueFree();
            if (Setting?.UI is PackedScene scene)
            {
                UI = scene.Instantiate();
                AddChild(UI);
            }
        }
    }
}
