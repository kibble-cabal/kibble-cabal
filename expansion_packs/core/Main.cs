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

        public static readonly RLocation[] Locations = [
            GD.Load<RLocation>("res://expansion_packs/core/location/resources/Island.tres")
        ];

        public static readonly RItem[] Items = [
            GD.Load<RItem>("res://expansion_packs/core/item/resources/Flower.tres"),
            GD.Load<RItem>("res://expansion_packs/core/item/resources/FoodBowl.tres"),
        ];

        public Main()
        {
            Locations.ForEach(LocationDB.Register);
            GameModes.ForEach(GameModeDB.Register);
            Settings.ForEach(SettingDefinitionDB.Register);
            Items.ForEach(ItemDB.Register);
        }
    }
}