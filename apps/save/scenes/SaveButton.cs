using Godot;

namespace KibbleCabal.Apps.Save
{
    public partial class SaveButton : Button
    {
        public async void OnPressed()
        {
            var successLabel = GetNode<Label>("SuccessLabel");
            SaveSubSystem.CommitChanges();
            successLabel.Visible = true;
            await ToSignal(GetTree().CreateTimer(2.0), Timer.SignalName.Timeout);
            successLabel.Visible = false;
        }
    }
}
