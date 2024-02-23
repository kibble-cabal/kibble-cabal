
using Godot;
using Query;
using Query.Filter;
using Query.Transformation;

namespace KibbleCabal.Core.Pet.AI.Task
{
    /// <summary>
    /// Fulfills the provided need, if the need value is low. Fails if the need value is too high, or if unable to fulfill the need.
    /// </summary>
    [Tool]
    public partial class BTTryFulfillNeed : BTFulfillNeed, IVerbose
    {
        private bool _verbose = false;
        private Attribute? NeedAttribute;
        private Tag? NeedProviderTag;
        private bool HasRunQuery = false;

        [Export(PropertyHint.ResourceType, "NeedAttribute")]
        public Resource? NeedAttributeResource
        {
            get => NeedAttribute?.Instance;
            set
            {
                if (value is not null) this.Set(ref NeedAttribute, new(value));
                else this.Set(ref NeedAttribute, null);
            }
        }

        [Export(PropertyHint.ResourceType, "Tag")]
        public Resource? NeedProviderTagResource
        {
            get => NeedProviderTag?.Instance;
            set
            {
                if (value is not null) this.Set(ref NeedProviderTag, new(value));
                else this.Set(ref NeedProviderTag, null);
            }
        }

        [Export]
        public bool Verbose
        {
            get => _verbose;
            set => _verbose = value;
        }

        public override string _GenerateName()
        {
            if (!string.IsNullOrEmpty(TargetItemVariable))
                return $"Fullfill {NeedAttribute?.Identifier} with item {TargetItemVariable}";
            return $"Fulfill {NeedAttribute?.Identifier}";
        }

        public override string[] _GetConfigurationWarnings()
        {
            var warnings = base._GetConfigurationWarnings();
            if (NeedAttribute is null) warnings = [.. warnings, "Missing Need Attribute!"];
            if (NeedProviderTag is null) warnings = [.. warnings, "Missing Need Provider Tag!"];
            return warnings;
        }

        public override Status _Tick(double delta)
        {
            // Fail if missing dependencies.
            var system = GetAbilitySystem();
            if (system is null) return this.FailWithWarning("Missing ability system.");
            if (NeedAttribute is null) return this.FailWithWarning("Missing need attribute.");
            if (NeedProviderTag is null) return this.FailWithWarning("Missing need provider tag.");
            if (!system.HasAttribute(NeedAttribute)) return this.FailWithWarning("Ability system is missing need attribute.");

            // Get value of need attribute.
            var needValue = system.GetAttributeValue(NeedAttribute);
            var needIsLow = needValue < (NeedAttribute.MaxValue - NeedAttribute.MinValue) / 2 + NeedAttribute.MinValue;

            // Fail if need value is above 50% (only when acting autonomously, not during instructions).
            if (!Blackboard.IsInstruction() && !needIsLow) return this.FailWithWarning($"Need {NeedAttribute.Identifier} is not low.");

            // Run query.
            if (!HasRunQuery && Agent is Node3D agent)
            {
                HasRunQuery = true;
                var query = BuildQuery();
                if (query is null) return this.FailWithWarning("Unable to build query.");
                var result = query.Run<PhysicsQuery.Result>(agent);
                if (!TargetItemVariable.IsEmpty()) Blackboard.SetVar(TargetItemVariable, result.Collider);
                else return this.FailWithWarning("No item to fulfill {NeedAttribute.Identifier} found.");
            }
            return base._Tick(delta);
        }

        protected PhysicsQuery? BuildQuery()
        {
            if (NeedAttribute is null || NeedProviderTag is null) return null;
            var pet = GetPet();
            var animal = pet?.GetAnimal();
            if (pet is null || animal is null) return null;
            return new PhysicsQuery
            {
                Region = new SphereShape3D() { Radius = animal.DetectionRadius },
                DetectBodies = true,
                DetectAreas = true,
                CollisionMask = (uint)Bit.Physics.Items,
                Filters = [
                    new FilterByTags {
                        TagsToCheck = NeedProviderTag is not null ? [NeedProviderTag.Instance] : []
                    }
                ],
                Transformations = [
                    new SortByDistanceTransformation { }.AsDynamic(),
                    new GetIndexTransformation { }.AsDynamic()
                ]
            };
        }
    }
}
