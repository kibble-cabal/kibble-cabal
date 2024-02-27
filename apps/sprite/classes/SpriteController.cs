using System.Threading.Tasks;
using Godot;

[GlobalClass]
public partial class SpriteController: Node3D
{
    private string? CurrentAnimation;
    private double CurrentTime;

    public override void _Process(double delta)
    {
        if (CurrentAnimation is not null)
            CurrentTime += delta;
    }

    public async Task Start(string animation)
    {
        SetProcess(true);
        
        // Transition from previous animation
        if (CurrentAnimation is not null)
            await _Transition(CurrentAnimation, CurrentTime, animation);
        
        // Reset state
        CurrentAnimation = animation;
        CurrentTime = 0;
        
        // Play animation
        await _Play(animation);
    }

    public async Task Stop()
    {
        await _Stop();
        CurrentAnimation = null;
        CurrentTime = 0;
        SetProcess(false);
    }

    protected virtual Task _Play(string animation) => Task.CompletedTask;
    protected virtual Task _Transition(string prevAnimation, double interruptTime, string nextAnimation) => Task.CompletedTask;
    protected virtual Task _Stop() => Task.CompletedTask;
}