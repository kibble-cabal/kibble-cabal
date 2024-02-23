using Godot;

namespace KibbleCabal.Apps.AI.Task
{
	/// <summary>
	/// Attempts to activate an ability on the provided AbilitySystem. Fails if ability is not activated.
	/// </summary>
	[Tool]
	public abstract partial class BTActivateAbility : BTAbilitySystem
	{
		public override string _GenerateName()
		{
			var ability = GetAbility();
			if (ability is not null) return $"Activate {ability.Identifier}";
			return "Activate ability";
		}

		public override Status _Tick(double delta)
		{
			var node = GetAbilitySystem();
			if (node is not null)
			{
				var ability = GetAbility();
				if (ability is not null && node.Activate(ability) is not null)
					return Status.Success;
			}
			return Status.Failure;
		}

		public abstract Ability? GetAbility();
	}
}
