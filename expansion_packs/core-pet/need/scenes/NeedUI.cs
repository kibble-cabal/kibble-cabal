using Godot;

namespace KibbleCabal.Core.Pet.Needs.UI
{
    public partial class NeedUI : HBoxContainer
    {
        private static readonly PackedScene Scene = GD.Load<PackedScene>("res://expansion_packs/core-pet/need/scenes/need_ui.tscn");

        public Attribute? Attribute;
        public AbilitySystem? AbilitySystem;

        [Export(PropertyHint.NodePathValidTypes, "Label")]
        public NodePath? LabelPath;

        [Export(PropertyHint.NodePathValidTypes, "ProgressBar")]
        public NodePath? ProgressBarPath;

        private Label? Label;
        private ProgressBar? ProgressBar;

        public static NeedUI Instantiate(AbilitySystem system, Attribute need)
        {
            var node = Scene.Instantiate<NeedUI>();
            node.AbilitySystem = system;
            node.Attribute = need;
            return node;
        }

        public override void _Ready()
        {
            ConnectAbilitySystem();
            ConnectAttribute();
            Update();
        }

        public void Update()
        {
            Label ??= GetNodeOrNull<Label>(LabelPath ?? default);
            ProgressBar ??= GetNodeOrNull<ProgressBar>(ProgressBarPath ?? default);
            if (AbilitySystem is null || Attribute is null || Label is null || ProgressBar is null) return;
            Label.Text = Attribute.Identifier.ToString().Replace("_", " ").Capitalize();
            ProgressBar.MinValue = Attribute.MinValue;
            ProgressBar.MaxValue = Attribute.MaxValue;
            ProgressBar.Step = (Attribute.MaxValue - Attribute.MinValue) / 100;
            ProgressBar.Value = AbilitySystem.GetAttributeValue(Attribute);
        }

        private void ConnectAbilitySystem() => AbilitySystem?.Instance.TryConnect("attributes_changed", Callable.From(Update));
        private void ConnectAttribute() => Attribute?.Instance.TryConnectChanged(Callable.From(Update));

    }
}
