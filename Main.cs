using Godot;

namespace KibbleCabal
{
    public partial class Main : Node3D
    {
        public override void _Ready()
        {
            SaveSubSystem.Instance.SaveChanged += OnSaveChanged;
            OnSaveChanged();
        }

        private void OnSaveChanged()
        {
            LocationSubSystem.To("Island");
        }
    }
}