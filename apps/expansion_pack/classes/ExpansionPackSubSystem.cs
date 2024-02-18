using Godot;
using Godot.Collections;

public sealed partial class ExpansionPackSubSystemBase : Node
{
    [Signal]
    public delegate void AllPacksInitializedEventHandler();

    [Signal]
    public delegate void PackInitializedEventHandler(RExpansionPack pack);

    public Dictionary<StringName, GodotObject?> InitializedExpansionPacks = [];

    public ExpansionPackSubSystemBase()
    {
        ExpansionPackLoader.LoadPacks().ForEach(ExpansionPackDB.Register);
        ExpansionPackDB.Resources.ForEach(Initialize);
        EmitSignal(SignalName.AllPacksInitialized);
    }

    public void Initialize(RExpansionPack pack)
    {
        if (!InitializedExpansionPacks.ContainsKey(pack.ID))
        {
            this.Print($"Initializing Expansion Pack: {pack.DisplayName}");
            InitializedExpansionPacks[pack.ID] = pack.EntryScript?.New();
            EmitSignal(SignalName.PackInitialized, [pack]);
        }
    }

    public override string ToString() => "ExpansionPackSubSystem";
}

public sealed partial class ExpansionPackSubSystem : Singleton<ExpansionPackSubSystemBase> { }