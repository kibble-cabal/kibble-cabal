using Godot;
using System;

namespace KibbleCabal.Apps.Settings.UI
{
    public partial class SettingsScene : VBoxContainer
    {
        public static readonly PackedScene Scene = GD.Load<PackedScene>("res://apps/settings/scenes/setting_scene.tscn");

        public override void _Ready()
        {
            SettingDefinitionDB.Resources.ForEach(Render);
            SettingDefinitionDB.Instance.Registered += (_, setting) => Render(setting);
        }

        public void Render(Resource setting) => Render((RSettingDefinition)setting);

        public void Render(RSettingDefinition setting)
        {
            var scene = Scene.Instantiate<SettingScene>();
            scene.Setting = setting;
            AddChild(scene);
            scene.Render();
        }
    }
}
