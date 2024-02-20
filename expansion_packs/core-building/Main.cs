
using Godot;

namespace KibbleCabal.Core.Building
{
    public partial class Main : GodotObject
    {
        public static readonly RGameMode[] GameModes = [
            GD.Load<RGameMode>("res://expansion_packs/core-building/game_mode/resources/BuildMode.tres"),
        ];

        public Main()
        {
            GameModes.ForEach(GameModeDB.Register);
        }
    }
}