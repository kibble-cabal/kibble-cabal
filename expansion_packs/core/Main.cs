using Godot;

namespace KibbleCabal.Core
{
    public partial class Main : GodotObject
    {
        public static readonly string BasePath = "res://expansion_packs/core";

        public static readonly RGameMode[] GameModes = [
            GD.Load<RGameMode>($"{BasePath}/game_mode/resources/LiveMode.tres"),
        ];

        public static readonly RSettingDefinition[] Settings = [
            GD.Load<RSettingDefinition>($"{BasePath}/settings/resources/ReduceMotion.tres")
        ];

        public static readonly RLocation[] Locations = [
            GD.Load<RLocation>($"{BasePath}/location/resources/Island.tres")
        ];

        public static readonly RItem[] Items = [
            GD.Load<RItem>($"{BasePath}/item/resources/Flower.tres"),
            GD.Load<RItem>($"{BasePath}/item/resources/FoodBowl.tres"),
        ];

        public static readonly RSubTree[] SubTrees = [
            GD.Load<RSubTree>($"{BasePath}/ai/resources/TestSubTree1.tres"),
            GD.Load<RSubTree>($"{BasePath}/ai/resources/TestSubTree2.tres"),
        ];

        public Main()
        {
            Locations.ForEach(LocationDB.Register);
            GameModes.ForEach(GameModeDB.Register);
            Settings.ForEach(SettingDefinitionDB.Register);
            Items.ForEach(ItemDB.Register);
            SubTrees.ForEach(SubTreeDB.Register);
        }
    }
}