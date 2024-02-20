using System.Collections.Generic;
using System.Linq;
using Godot;

public sealed partial class SaveSubSystemBase : Node
{
    [Signal]
    public delegate void SaveOpenedEventHandler(RSave save);

    [Signal]
    public delegate void SaveClosedEventHandler(RSave save);

    [Signal]
    public delegate void SaveChangedEventHandler();

    [Signal]
    public delegate void BeforeSavedEventHandler();

    [Signal]
    public delegate void AfterSavedEventHandler();

    private Timer Timer = new();

    public RSave? Current { get; private set; }

    public override void _EnterTree()
    {
        Timer.Autostart = true;
        Timer.WaitTime = 5.0f;
        Timer.Connect(Timer.SignalName.Timeout, Callable.From(CommitChanges));
        AddChild(Timer);
        Open(DiscoverSaves().FirstOrDefault() ?? new());
    }

    public void CommitChanges()
    {
        GD.PrintS($"[SaveSubSystem] Saving.");
        if (Current is RSave save)
            save.CommitChanges();
    }

    public void Open(RSave save)
    {
        if (Current != null) Close();
        Current = save;
        save.BeforeSaved += EmitBeforeSaved;
        save.AfterSaved += EmitAfterSaved;
        EmitSignal(SignalName.SaveOpened, [save]);
        EmitSignal(SignalName.SaveChanged);
    }

    public void Close()
    {
        if (Current is RSave save)
        {
            save.BeforeSaved -= EmitBeforeSaved;
            save.AfterSaved -= EmitAfterSaved;
            Current = null;
            EmitSignal(SignalName.SaveClosed, [save]);
            EmitSignal(SignalName.SaveChanged);
        }
    }

    public T GetSetting<[MustBeVariant] T>(StringName id)
    {
        if (Current is not null) return Current.Settings.Get<T>(id);
        return default!;
    }

    public void ChangeSetting<[MustBeVariant] T>(StringName id, T value) => Current?.Settings.Change(id, value);

    private void EmitBeforeSaved() => EmitSignal(SignalName.BeforeSaved);
    private void EmitAfterSaved() => EmitSignal(SignalName.AfterSaved);

    public static IEnumerable<RSave> DiscoverSaves() => DirAccess.DirExistsAbsolute(RSave.BaseDir)
        ? DirAccess.GetFilesAt(RSave.BaseDir)
            .Select(path => GD.Load<RSave>(RSave.BaseDir.PathJoin(path)))
            .WhereNotNull()
        : [];
}

public sealed partial class SaveSubSystem : Singleton<SaveSubSystemBase>
{
    public static RSave? Current => Instance.Current;
    public static void Open(RSave save) => Instance.Open(save);
    public static void Close() => Instance.Close();
    public static void CommitChanges() => Instance.CommitChanges();
    public static T GetSetting<[MustBeVariant] T>(StringName id) => Instance.GetSetting<T>(id);
    public static void ChangeSetting<[MustBeVariant] T>(StringName id, T value) => Instance.ChangeSetting<T>(id, value);
    public static IEnumerable<RSave> DiscoverSaves() => SaveSubSystemBase.DiscoverSaves();
}