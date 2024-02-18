using Godot;
using Godot.Collections;

public sealed partial class ModSubSystemBase : Node
{
    [Signal]
    public delegate void ModInitializedEventHandler(RMod mod);

    [Signal]
    public delegate void AllModsInitializedEventHandler();

    public Array<StringName> InitializedMods { get; private set; } = [];

    public ModSubSystemBase()
    {
        ModLoader.LoadMods().ForEach(ModDB.Register);
        ModDB.Resources.ForEach(Initialize);
        ModDB.Instance.Registered += Initialize;
    }

    public void Initialize(Resource resource) => Initialize((RMod)resource);
    public void Initialize(RMod mod)
    {
        if (!InitializedMods.Contains(mod.ID))
        {
            InitializedMods.Add(mod.ID);
            // TODO (when Lua is added):
            // mod.RunEntryScript(); 
            EmitSignal(SignalName.ModInitialized, [mod]);
        }
    }
}

public sealed partial class ModSubSystem : Singleton<ModSubSystemBase> { }