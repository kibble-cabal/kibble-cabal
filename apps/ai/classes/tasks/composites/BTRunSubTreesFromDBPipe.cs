using Godot;

namespace KibbleCabal.Apps.AI.Task
{
    /// <summary>
    /// Runs behavior trees from the behavior tree database.
    /// </summary>
    [Tool]
    public partial class BTRunSubTreesFromDBPipe : BTPipe, IBTRunSubTreesFromDB
    {
        private StringName _hookKey = "";

        [Export]
        public StringName HookKey
        {
            get => _hookKey;
            set => this.Set(ref _hookKey, value);
        }

        public override string _GenerateName() => (this as IBTRunSubTreesFromDB).GetName();

        public override void _Setup() => IBTRunSubTreesFromDB.Setup(this);
    }
}
