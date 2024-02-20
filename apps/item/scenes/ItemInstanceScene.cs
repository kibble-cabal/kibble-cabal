using Godot;

namespace KibbleCabal.Apps.Item
{
    public partial class ItemInstanceScene : Node3D
    {
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
    }
}