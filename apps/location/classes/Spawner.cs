using Godot;

public abstract partial class Spawner : Resource
{
    public const string GroupName = "Spawned";
    public const string MetaName = "Spawner";
    public const string TopLevelGroupName = "SpawnedTopLevel";

    public Spawner() { }
    public Spawner(Resource resource) => SetResource(resource);
    public abstract Resource? GetResource();
    public abstract void SetResource(Resource? value);
    public abstract bool HasSpawned();
    public abstract bool IsSubspawner();
    public abstract void Spawn(Node3D world);
    public abstract void Update();
    public abstract void Despawn();
}

public abstract partial class Spawner<R, N> : Spawner where R : Resource where N : Node
{
    private R? Resource { get; set; }

    /// <summary>
    /// Exposes the provided resource to Godot's save system.
    /// </summary>
    [Export]
    private Resource? Value
    {
        get => Resource;
        set => SetResource(value as R);
    }

    private N? Node;

    public Spawner() { }
    public Spawner(R resource) => SetResource(resource);

    protected virtual N? _Spawn(R resource, Node3D world) => null;
    protected virtual void _Update(R resource, N node) { }
    protected virtual void _Despawn(N node)
    {
        if (node.CanQueueFree())
            node.QueueFree();
    }
    protected virtual bool _IsSubspawner() => false;

    public override sealed R? GetResource() => Resource;
    public override sealed void SetResource(Resource? value)
    {
        Resource?.TryDisconnectChanged(Callable.From(Update));
        value?.TryConnectChanged(Callable.From(Update));
        Resource = value as R;
        EmitChanged();
    }

    public override sealed bool HasSpawned() => Node != null;
    public override sealed bool IsSubspawner() => _IsSubspawner();

    public override sealed void Spawn(Node3D world)
    {
        if (Resource is R resource)
            Node = _Spawn(resource, world);
        if (Node is N node)
        {
            node.SetMeta(MetaName, this);
            node.AddToGroup(GroupName);
            if (!IsSubspawner()) node.AddToGroup(TopLevelGroupName);
        }
        Update();
    }

    public override sealed void Update()
    {
        if (Node is N node && Resource is R resource)
            _Update(resource, node);
    }

    public override sealed void Despawn()
    {
        if (Resource == null) return;
        if (Node is N node)
            _Despawn(node);
        Node = null;
    }
}

public interface ISpawnable
{
    Spawner GetSpawner();
}