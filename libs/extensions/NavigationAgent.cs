using Godot;

public static class NavigationAgentExtensions
{
    public static void Stop(this NavigationAgent3D agent)
    {
        if (agent.IsNavigationFinished()) return;
        var parent = agent.GetParentOrNull<Node3D>();
        if (parent is not null)
        {
            agent.TargetPosition = parent.GlobalPosition;
            agent.Velocity = Vector3.Zero;
        }
    }
}