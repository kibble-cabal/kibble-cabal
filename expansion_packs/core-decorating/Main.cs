using Godot;
using System.Collections.Generic;

namespace KibbleCabal.Core.Decorating
{
    public partial class Main : GodotObject
    {
        
        public static readonly string BasePath = "res://expansion_packs/core-decorating";
        
        private static readonly IEnumerable<RGameMode> GameModes =
        [
            GD.Load<RGameMode>($"{BasePath}/game_mode/resources/DecorateMode.tres")
        ];

        public Main()
        {
            GameModes.ForEach(GameModeDB.Register);
        }
    }
}
