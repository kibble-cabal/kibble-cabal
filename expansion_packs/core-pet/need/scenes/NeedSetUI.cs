using Godot;
using System.Collections.Generic;
using System.Linq;
using AS;

namespace KibbleCabal.Core.Pet.Needs.UI
{
    public partial class NeedSetUI : VBoxContainer
    {
        public AbilitySystem? _abilitySystem;

        public AbilitySystem? AbilitySystem
        {
            get => _abilitySystem;
            set
            {
                _abilitySystem = value;
                ConnectAbilitySystem();
            }
        }

        [Export]
        public NodePath AbilitySystemPath = "";

        private readonly Dictionary<Attribute, NeedUI> Nodes = [];

        public override void _Ready()
        {
            var node = GetNodeOrNull(AbilitySystemPath);
            if (node is not null)
                AbilitySystem = new AbilitySystem(node);
        }

        public void Update()
        {
            if (AbilitySystem is null || !IsInsideTree()) return;
            var attributes = NeedsConfig.Instance.NeedAttributes.Where(AbilitySystem.HasAttribute);

            // Add new attributes
            attributes.Except(Nodes.Keys).ForEach(attribute =>
            {
                var scene = NeedUI.Instantiate(AbilitySystem, attribute);
                Nodes[attribute] = scene;
                AddChild(scene);
            });

            // Remove outdated attributes
            Nodes.Keys.Except(attributes).Select(attr => Nodes[attr]).QueueFreeAll();
        }

        private void ConnectAbilitySystem()
        {
            AbilitySystem?.Instance?.TryConnect("attributes_changed", Callable.From(Update));
            Update();
        }

    }
}
