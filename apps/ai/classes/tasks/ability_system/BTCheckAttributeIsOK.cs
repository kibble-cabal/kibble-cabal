using Godot;
using AS;

namespace KibbleCabal.Apps.AI.Task
{
	/// <summary>
	/// Succeeds if the provided attribute is above 50%.
	/// </summary>
	[Tool]
	public partial class BTCheckAttributeIsOK : BTAbilitySystem
	{
		private Attribute? Attribute;

		[Export(PropertyHint.ResourceType, "Attribute")]
		public Resource? AttributeResource
		{
			get => Attribute?.Instance;
			set => this.Set(ref Attribute, value);
		}

		public override string[] _GetConfigurationWarnings()
		{
			var warnings = base._GetConfigurationWarnings();
			if (Attribute is null) warnings = [.. warnings, "Missing Attribute!"];
			return warnings;
		}

		public override Status _Tick(double delta)
		{
			var node = GetAbilitySystem();
			if (node is null || Attribute is null) return Status.Failure;
			if (!node.HasAttribute(Attribute)) return Status.Failure;
			if (node.IsAttributeLow(Attribute)) return Status.Failure;
			return Status.Success;
		}
	}
}
