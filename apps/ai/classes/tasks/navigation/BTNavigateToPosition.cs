using Godot;

namespace KibbleCabal.Apps.AI.Task
{
    /// <summary>
    /// Sets the target position for the given NavigationAgent.
    /// If the navigation stops but the target is not reached, this task fails.
    /// </summary>
    [Tool]
    public partial class BTNavigateToPosition : BTNavigate
    {
        [Export]
        public BBVector3? TargetPosition;

        [Export]
        public BBFloat? MaxDistance;

        public override string _GenerateName() => $"Navigate to {TargetPosition}";

        protected override Vector3 GetNavigationPosition() => (Vector3)(TargetPosition?.GetValue(Agent, Blackboard) ?? default);
        protected override float GetMaxDistance() => (float)(MaxDistance?.GetValue(Agent, Blackboard) ?? default);
    }
}