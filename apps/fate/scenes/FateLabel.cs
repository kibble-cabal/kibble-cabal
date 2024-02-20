using Godot;

namespace KibbleCabal.Apps.Fate
{
    public partial class FateLabel : Label
    {
        private static RFate? Fate => SaveSubSystem.Current?.Fate;
        private static int Amount => Fate?.Amount ?? 0;

        public FateLabel() => SaveSubSystem.Instance.SaveChanged += OnSaveChanged;

        public override void _Ready() => Update();

        private void OnSaveChanged()
        {
            Fate?.TryConnectChanged(Callable.From(Update));
            Update();
        }

        private void Update() => Text = $"{Amount}f";
    }
}
