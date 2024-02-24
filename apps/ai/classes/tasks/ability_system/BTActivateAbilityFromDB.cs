using Godot;
using AS;

namespace KibbleCabal.Apps.AI.Task
{
	/// <summary>
	/// Activates ability from the ability database.
	/// </summary>
	[Tool]
	public partial class BTActivateAbilityFromDB : BTActivateAbility
	{
		private StringName _identifier = "";

		[Export]
		public StringName AbilityIdentifier
		{
			get => _identifier;
			set => this.Set(ref _identifier, value);
		}

		public override string[] _GetConfigurationWarnings()
		{
			var warnings = base._GetConfigurationWarnings();
			if (string.IsNullOrEmpty(AbilityIdentifier)) warnings = [.. warnings, "Ability Identifier can't be empty!"];
			return warnings;
		}

		public override Ability? GetAbility() => AbilityDB.Find(AbilityIdentifier);
	}
}
