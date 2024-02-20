using Godot;

namespace KibbleCabal.Apps.Save
{
    public partial class SavesList : VBoxContainer
    {
        public override void _Ready() => SaveSubSystem.DiscoverSaves().ForEach(Render);

        private void Render(RSave save)
        {
            var button = new Button
            {
                Text = $"Save {save.ID}"
            };
            button.Pressed += () => SaveSubSystem.Open(save);
            AddChild(button);
        }

        public void OnNewSaveButtonPressed()
        {
            var newSave = new RSave();
            SaveSubSystem.Open(newSave);
            SaveSubSystem.CommitChanges();
            Render(newSave);
        }
    }
}