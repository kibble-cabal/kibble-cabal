using Godot;

public interface ISubSystem
{
    Node _Node { get => (Node)this; }
}

public interface ISaveFileSubSystem
{
    void _OnBeforeSaveFileEntered() { }
    void _OnAfterSaveFileEntered() { }
    void _OnBeforeSaveFileExited() { }
    void _OnAfterSaveFileExited() { }
}

public interface IDependentSubSystem : ISubSystem
{
    ISubSystem[] Dependencies { get; }

    void InitializeDependencies() => Dependencies.ForEach(dependency => dependency._Node.Connect(
        Node.SignalName.Ready,
        Callable.From(() => OnDependencyReady(dependency)),
        (uint)GodotObject.ConnectFlags.OneShot
    ));

    void OnDependencyReady(ISubSystem dependency) { }
    void OnDependenciesReady() { }
}