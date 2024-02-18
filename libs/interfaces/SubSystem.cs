using System.Linq;
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

public interface INode
{
    public void _Ready();
}

public interface IDependentSubSystem : ISubSystem
{
    ISubSystem[] Dependencies { get; }

    void InitializeDependencies()
    {
        if (AreDependenciesReady)
            OnDependenciesReady();
        Dependencies.ForEach(dependency => dependency._Node.Connect(
            Node.SignalName.Ready,
            Callable.From(() =>
            {
                OnDependencyReady(dependency);
                if (AreDependenciesReady)
                    OnDependenciesReady();
            })
        ));
    }

    bool AreDependenciesReady => Dependencies.All(dependency => dependency._Node.IsNodeReady());

    void OnDependencyReady(ISubSystem dependency) { }
    void OnDependenciesReady() { }
}

public abstract partial class DependentSubSystem : Node, IDependentSubSystem
{
    public abstract ISubSystem[] Dependencies { get; }

    public DependentSubSystem() => (this as IDependentSubSystem).InitializeDependencies();

    public virtual void OnDependenciesReady() { }
    public virtual void OnDependencyReady(ISubSystem dependency) { }
}