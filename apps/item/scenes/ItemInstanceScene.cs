using System.Linq;
using Godot;
using AS;

namespace KibbleCabal.Apps.Item
{
    public partial class ItemInstanceScene : Node3D
    {
        public static readonly PackedScene Scene = GD.Load<PackedScene>("res://apps/item/scenes/item_instance_scene.tscn");

        private AbilitySystem? AbilitySystem;

        [Export]
        public RItemInstance? ItemInstance;

        public override void _Ready()
        {
            var item = ItemInstance?.GetItem();
            
            // Set ability system state
            AbilitySystem = new AbilitySystem(GetNode("AbilitySystem"));
            if (AbilitySystem is AbilitySystem system)
            {
                item?.AbilitySystemState.MergeInto(system);
                ItemInstance?.AbilitySystemState?.MergeInto(system);
            }

            // Add item scene
            if (item?.Physics?.Scene is { } scene)
                AddChild(scene.Instantiate());

            SaveSubSystem.Instance.BeforeSaved += OnBeforeSave;
        }

        private void OnBeforeSave()
        {
            // Update ability system state
            if (AbilitySystem is not null) ItemInstance?.AbilitySystemState?.MergeWith(AbilitySystem);
        }

        public static ItemInstanceScene Instantiate(RItemInstance itemInstance)
        {
            var node = Scene.Instantiate<ItemInstanceScene>();
            node.ItemInstance = itemInstance;
            return node;
        }
    }
}