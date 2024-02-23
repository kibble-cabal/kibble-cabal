
using Godot;
using KibbleCabal.Apps.AI.Task;

namespace KibbleCabal.Core.Pet.AI.Task
{
    /// <summary>
    /// Fulfills the provided need.
    /// </summary>
    [Tool]
    public partial class BTTryFulfillNeed : BTFulfillNeed
    {
        private Attribute? _needAttribute;
        private Tag? _needProviderTag;

        [Export]
        public Resource? NeedAttribute
        {
            get => _needAttribute?.Instance;
            set
            {
                if (value is not null) this.Set(ref _needAttribute, new(value));
                else this.Set(ref _needAttribute, null);
            }
        }

        [Export]
        public Resource? NeedProviderTag
        {
            get => _needProviderTag?.Instance;
            set
            {
                if (value is not null) this.Set(ref _needProviderTag, new(value));
                else this.Set(ref _needProviderTag, null);
            }
        }

        public override string _GenerateName()
        {
            if (!string.IsNullOrEmpty(TargetItemVariable))
                return $"Fullfill {_needAttribute?.Identifier} with item {TargetItemVariable}";
            return $"Fulfill {_needAttribute?.Identifier}";
        }

        public override string[] _GetConfigurationWarnings()
        {
            var warnings = base._GetConfigurationWarnings();
            if (NeedAttribute is null) warnings = [.. warnings, "Missing Need Attribute!"];
            if (NeedProviderTag is null) warnings = [.. warnings, "Missing Need Provider Tag!"];
            return warnings;
        }

        protected Resource? BuildQuery()
        {
            if (_needAttribute is null || _needProviderTag is null) return null;

            var pet = GetPet();
            var animal = pet?.GetAnimal();

            if (pet is null || animal is null) return null;

            var region = new SphereShape3D() { Radius = animal.DetectionRadius };

            // TODO
            return null;
        }
    }
}
