using Godot;
using AS;
using System.Linq;

namespace KibbleCabal.Apps.AI.Task
{
	/// <summary>
	/// Succeeds if the provided ability is active.
	/// </summary>
	[Tool]
	public partial class BTCheckAbilityIsActivated : BTAbilitySystem
	{
		private Ability? Ability;

		[Export(PropertyHint.ResourceType, "Ability")]
		public Resource? AbilityResource
		{
			get => Ability?.Instance;
			set => this.Set(ref Ability, value);
		}

		public override string[] _GetConfigurationWarnings()
		{
			var warnings = base._GetConfigurationWarnings();
			if (Ability is null) warnings = [.. warnings, "Missing Ability!"];
			return warnings;
		}

		public override Status _Tick(double delta)
		{
			var node = GetAbilitySystem();
			if (node is null || Ability is null) return Status.Failure;
			if (node.Events.Any(e => e.Ability == Ability))
				return Status.Success;
			return Status.Failure;
		}
	}
}
