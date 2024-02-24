using Godot;
using AS;

namespace KibbleCabal.Apps.AI.Task
{
    /// <summary>
    /// Succeeds if the provided attribute is above 50%.
    /// </summary>
    [Tool]
    public partial class BTSetVarFromAttribute : BTAbilitySystem
    {
        private Attribute? Attribute;
        private string _blackboardVar = "";

        [Export(PropertyHint.ResourceType, "Attribute")]
        public Resource? AttributeResource
        {
            get => Attribute?.Instance;
            set => this.Set(ref Attribute, value);
        }

        [Export]
        public string BlackboardVar
        {
            get => _blackboardVar;
            set => this.Set(ref _blackboardVar, value);
        }

        public override string _GenerateName() => $"Set {BlackboardVar} from attribute \"{Attribute?.Identifier}\"";

        public override string[] _GetConfigurationWarnings()
        {
            var warnings = base._GetConfigurationWarnings();
            if (Attribute is null) warnings = [.. warnings, "Missing Attribute!"];
            return warnings;
        }

        public override Status _Tick(double delta)
        {
            var node = GetAbilitySystem();
            if (node is null || Attribute is null || BlackboardVar.IsEmpty()) return Status.Failure;
            if (!node.HasAttribute(Attribute)) return Status.Failure;
            Blackboard.SetVar(BlackboardVar, node.GetAttributeValue(Attribute));
            return Status.Success;
        }
    }
}
