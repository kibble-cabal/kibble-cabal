using Godot;

namespace KibbleCabal.Apps.Item
{
    public partial class ItemInstanceScene : Node3D
    {
        public static readonly PackedScene Scene = GD.Load<PackedScene>("res://apps/item/scenes/item_instance_scene.tscn");

        private Node? AbilitySystem;

        [Export]
        public RItemInstance? ItemInstance;

        public override void _Ready()
        {
            var item = ItemInstance?.GetItem();

            // TODO: ability state
            AbilitySystem = GetNode<Node>("AbilitySystem");

            // Add instance scene
            var scene = item?.Physics?.Scene;
            if (scene is not null)
                AddChild(scene.Instantiate());
        }

        public static ItemInstanceScene Instantiate(RItemInstance itemInstance)
        {
            var node = Scene.Instantiate<ItemInstanceScene>();
            node.ItemInstance = itemInstance;
            return node;
        }
    }
}