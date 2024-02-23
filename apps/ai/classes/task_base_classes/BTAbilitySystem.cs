using Godot;

namespace KibbleCabal.Apps.AI.Task
{
	/// <summary>
	/// Base class for tasks that use the ability system.
	/// </summary>
	[Tool]
	public abstract partial class BTAbilitySystem : BTAction
	{
		[Export]
		public BBNode? AbilitySystemPath;

		public override string[] _GetConfigurationWarnings()
		{
			if (AbilitySystemPath is null) return ["Missing Ability System!"];
			return [];
		}

		protected AbilitySystem? GetAbilitySystem()
		{
			if (AbilitySystemPath is not null)
			{
				var node = AbilitySystemPath.GetValue(Agent, Blackboard).TryAs<Node>();
				if (node is not null) return new(node);
			}
			return null;
		}
	}
}
