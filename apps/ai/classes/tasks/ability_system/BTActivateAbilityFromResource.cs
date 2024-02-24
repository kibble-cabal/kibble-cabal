using Godot;
using AS;

namespace KibbleCabal.Apps.AI.Task
{
    /// <summary>
    /// Activates provided ability.
    /// </summary>
    [Tool]
    public partial class BTActivateAbilityFromResource : BTActivateAbility
    {
        private Ability? Ability;

        [Export(PropertyHint.ResourceType, "Ability")]
        public Resource? AbilityResource
        {
            get => Ability?.Instance;
            set
            {
                if (value is not null) this.Set(ref Ability, new(value));
                else this.Set(ref Ability, null);
            }
        }

        public override string[] _GetConfigurationWarnings()
        {
            var warnings = base._GetConfigurationWarnings();
            if (AbilityResource is null) warnings = [.. warnings, "Ability not provided!"];
            return warnings;
        }

        public override Ability? GetAbility() => Ability;
    }
}
