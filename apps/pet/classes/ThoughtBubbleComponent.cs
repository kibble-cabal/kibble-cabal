using System.Linq;
using Godot;
using System.Threading.Tasks;

namespace KibbleCabal.Apps.Pet;

public readonly struct ThoughtBubbleComponent(Node parent)
{
    public async Task SpawnThoughtBubble(string text, float duration = 3, float maxWidth = -1)
    {
        await DestroyThoughtBubbles();
        var bubble = new ThoughtBubble(text, duration, maxWidth)
        {
            LocalPosition = Vector3.Zero,
            ScreenOffset = new Vector2(maxWidth / 2, -20)
        };
        parent.AddChild(bubble);
    }
    
    public static void DestroyThoughtBubble(ThoughtBubble bubble)
    {
        if (!bubble.CanQueueFree()) return;
        _ = bubble.Destroy();
    }

    public async Task DestroyThoughtBubbles()
    {
        parent.GetChildren()
            .Where(child => child.IsInGroup(ThoughtBubble.GroupName))
            .ForEach(child => DestroyThoughtBubble((ThoughtBubble)child));
        await parent.ToSignal(parent.GetTree().CreateTimer(0.25), Timer.SignalName.Timeout);
    }
}