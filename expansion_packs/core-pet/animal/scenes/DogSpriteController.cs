using System.Threading.Tasks;
using Godot;

namespace KibbleCabal.Core.Pet
{
    /// <summary>
    /// This is an example class to show how the sprite controller framework works.
    /// </summary>
    public partial class DogSpriteController : SpriteController
    {
        [Export] AnimatedSprite3D? Sprite;

        protected override async Task _Play(string animation)
        {
            await base._Play(animation);
            Sprite?.Play(animation);
        }

        protected override async Task _Transition(string prevAnimation, double interruptTime, string nextAnimation)
        {
            await base._Transition(prevAnimation, interruptTime, nextAnimation);
            if (Sprite is null) return;
            var tweenA = GetTree().CreateTween().TweenProperty(Sprite, "modulate", Colors.Yellow, 0.125f);
            await ToSignal(tweenA, Tween.SignalName.Finished);
            var tweenB = GetTree().CreateTween().TweenProperty(Sprite, "modulate", Colors.White, 0.125f);
            await ToSignal(tweenB, Tween.SignalName.Finished);
        }

        protected override async Task _Stop()
        {
            await base._Stop();
            Sprite?.Stop();
        }
    }
}