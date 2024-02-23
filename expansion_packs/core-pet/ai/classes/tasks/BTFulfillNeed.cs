
using Godot;
using KibbleCabal.Apps.AI.Task;
using KibbleCabal.Apps.Pet;

namespace KibbleCabal.Core.Pet.AI.Task
{
    /// <summary>
    /// Fulfills the provided need.
    /// </summary>
    [Tool]
    public partial class BTFulfillNeed : BTNavigate
    {

        StringName _targetItemVariable = "";
        BBNode? _abilitySystemPath;
        Ability? _fulfillNeedAbility;

        bool HasNavigated = false;
        AbilityEvent? Event;
        bool HasFinishedEvent = false;

        [Export]
        public StringName TargetItemVariable
        {
            get => _targetItemVariable;
            set => this.Set(ref _targetItemVariable, value);
        }

        [Export]
        public BBNode? AbilitySystemPath
        {
            get => _abilitySystemPath;
            set => this.Set(ref _abilitySystemPath, value);
        }

        [Export]
        public Resource? FulfillNeedAbility
        {
            get => _fulfillNeedAbility?.Instance;
            set
            {
                if (value is not null) this.Set(ref _fulfillNeedAbility, new(value));
                else this.Set(ref _fulfillNeedAbility, null);
            }
        }

        public override string[] _GetConfigurationWarnings()
        {
            if (AbilitySystemPath is null) return ["Missing Ability System!"];
            return [];
        }

        public override Status _Tick(double delta)
        {
            var abilitySystem = GetAbilitySystem();
            if (abilitySystem is null || _fulfillNeedAbility is null) return Status.Failure;

            // If navigation is not finished, continue navigating.
            if (GetTargetItem() is not null && !HasNavigated)
            {
                var status = base._Tick(delta);
                switch (status)
                {
                    case Status.Success:
                        HasNavigated = true;
                        return Status.Running;
                    case Status.Failure:
                        GD.PushWarning($"Unable to navigate to {GetNavigationPosition()}");
                        return Status.Failure;
                    default:
                        return status;
                }
            }

            // If the event is not already started, activate it.
            if (Event is null)
            {
                Event = abilitySystem.Activate(_fulfillNeedAbility);
                if (Event is null)
                    abilitySystem.Instance.Connect("ability_event_finished", new Callable(this, MethodName.OnAbilityEventFinished));
                else
                {
                    GD.PushWarning($"Unable to activate {_fulfillNeedAbility.Identifier}");
                    return Status.Failure;
                }
            }

            // If the event is finished, the task has succeeded.
            if (HasFinishedEvent) return Status.Success;

            return Status.Running;
        }

        protected Node3D? GetTargetItem() => Blackboard.GetVar(TargetItemVariable).TryAs<Node3D>();

        protected override Vector3 GetNavigationPosition() => GetTargetItem()?.GlobalPosition ?? default;

        protected RPet? GetPet()
        {
            if (Agent is PetScene pet) return pet.Resource;
            return null;
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

        protected void OnAbilityEventFinished(Resource finishedEvent)
        {
            if (finishedEvent == Event?.Instance)
            {
                HasFinishedEvent = true;
                GetAbilitySystem()?.Instance.DisconnectAllFromTarget("ability_event_finished", this);
            }
        }
    }
}
