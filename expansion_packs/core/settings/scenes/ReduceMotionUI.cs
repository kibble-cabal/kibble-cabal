using Godot;

namespace KibbleCabal.Core.Settings
{
    public partial class ReduceMotionUI : CheckBox
    {
        public static readonly StringName ID = "Accessibility/ReduceMotion";
        public override void _Ready()
        {
            ButtonPressed = SaveSubSystem.GetSetting<bool>(ID);
            Connect(BaseButton.SignalName.Toggled, new Callable(this, "OnToggled"));
        }

        private void OnToggled(bool value) => SaveSubSystem.ChangeSetting<bool>(ID, value);
    }
}