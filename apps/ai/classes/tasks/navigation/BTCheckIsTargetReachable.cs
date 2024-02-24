using Godot;

namespace KibbleCabal.Apps.AI.Task
{
    /// <summary>
    /// Succeeds if the target is reachable for a given NavigationAgent.
    /// </summary>
    [Tool]
    public partial class BTCheckIsTargetReachable : BTAction
    {
        [Export]
        public BBNode? NavigationAgent;

        public override string _GenerateName() => $"Is Target Reachable?";

        public override Status _Tick(double delta)
        {
            var navigationAgent = NavigationAgent?.GetValue(Agent, Blackboard).TryAs<NavigationAgent3D>();
            if (navigationAgent is null) return Status.Failure;
            return navigationAgent.IsTargetReachable().AsStatus();
        }
    }
}