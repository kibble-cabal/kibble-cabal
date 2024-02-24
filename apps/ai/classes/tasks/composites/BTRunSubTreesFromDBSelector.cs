using Godot;

namespace KibbleCabal.Apps.AI.Task
{
    /// <summary>
    /// Runs behavior trees from the behavior tree database.
    /// </summary>
    [Tool]
    public partial class BTRunSubTreesFromDBSelector : BTSelector, IBTRunSubTreesFromDB
    {
        StringName _hookKey = "";

        [Export]
        public StringName HookKey
        {
            get => _hookKey;
            set => this.Set(ref _hookKey, value);
        }

        public override string _GenerateName() => this.GetName();
        public override void _Setup() => this.Setup();
    }
}
