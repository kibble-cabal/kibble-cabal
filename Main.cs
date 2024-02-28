using Godot;

namespace KibbleCabal
{
    public partial class Main : Node3D
    {
        [Export]
        public NavigationRegion3D? NavigationRegion;
        
        public override void _Ready()
        {
            SaveSubSystem.Instance.SaveChanged += OnSaveChanged;
            LocationSubSystem.Instance.LocationChanged += OnLocationChanged;
            OnSaveChanged();
            
            JSON.Schema.GeneratorDB.Generate();
        }

        private void OnSaveChanged()
        {
            LocationSubSystem.To("Island");
        }

        private void OnLocationChanged()
        {
            NavigationRegion?.BakeNavigationMesh();
        }
    }
}