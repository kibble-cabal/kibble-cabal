using Godot;

namespace KibbleCabal.Core
{
    public partial class Main : GodotObject
    {
        public static readonly RGameMode[] GameModes = [
            GD.Load<RGameMode>("res://expansion_packs/core/game_mode/resources/LiveMode.tres"),

        ];

        public static readonly RSettingDefinition[] Settings = [
            GD.Load<RSettingDefinition>("res://expansion_packs/core/settings/resources/ReduceMotion.tres")
        ];

        public Main()
        {
            GameModes.ForEach(GameModeDB.Register);
            Settings.ForEach(SettingDefinitionDB.Register);
        }
    }
}