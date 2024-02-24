using Godot;

namespace KibbleCabal.Apps.AI.Task
{
    /// <summary>
    /// Succeeds if navigation is finished for a given NavigationAgent.
    /// </summary>
    [Tool]
    public partial class BTCheckIsNavigationFinished : BTAction
    {
        [Export]
        public BBNode? NavigationAgent;

        public override string _GenerateName() => $"Is Navigation Finished?";

        public override Status _Tick(double delta)
        {
            var navigationAgent = NavigationAgent?.GetValue(Agent, Blackboard).TryAs<NavigationAgent3D>();
            if (navigationAgent is null) return Status.Failure;
            return navigationAgent.IsNavigationFinished().AsStatus();
        }
    }
}