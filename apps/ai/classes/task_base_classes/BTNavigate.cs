using Godot;

namespace KibbleCabal.Apps.AI.Task
{
    /// <summary>
    /// Base class to use NavigationAgent3D.
    /// </summary>
    [Tool]
    public abstract partial class BTNavigate : BTAction
    {
        private BBNode? _navigationAgentPath;

        [Export]
        public BBNode? NavigationAgentPath
        {
            get => _navigationAgentPath;
            set => this.Set(ref _navigationAgentPath, value);
        }

        private NavigationAgent3D? NavigationAgent;

        public override string[] _GetConfigurationWarnings()
        {
            if (NavigationAgentPath is null) return ["Missing navigation agent!"];
            return [];
        }

        public override void _Enter()
        {
            NavigationAgent = NavigationAgentPath?.GetValue(Agent, Blackboard).TryAs<NavigationAgent3D>();
            if (NavigationAgent is not null)
                NavigationAgent.TargetPosition = GetNavigationPosition();
        }

        public override void _Exit() => NavigationAgent?.Stop();

        public override Status _Tick(double delta)
        {
            if (NavigationAgent is null) return Status.Failure;
            if (!NavigationAgent.IsTargetReachable()) return Status.Failure;
            if (NavigationAgent.IsNavigationFinished())
            {
                if (NavigationAgent.IsTargetReached())
                    return Status.Success;
                return Status.Failure;
            }
            if (NavigationAgent.DistanceToTarget() < GetMaxDistance())
            {
                NavigationAgent.Stop();
                return Status.Success;
            }
            return Status.Running;
        }

        protected abstract Vector3 GetNavigationPosition();
        protected virtual float GetMaxDistance() => 0.0f;
    }
}