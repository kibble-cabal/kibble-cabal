using Godot;

public abstract partial class SpawnerBase : Resource
{
	protected const string GroupName = "Spawned";
	public const string MetaName = "Spawner";
	public const string TopLevelGroupName = "SpawnedTopLevel";

	protected SpawnerBase() { }
	public SpawnerBase(Resource resource) => SetResource(resource);
	public abstract Resource? GetResource();
	protected abstract void SetResource(Resource? value);
	public abstract bool HasSpawned();
	protected abstract bool IsSubSpawner();
	public abstract void Spawn(Node3D world);
	protected abstract void Update();
	public abstract void Despawn();
}

public abstract partial class Spawner<R, N> : SpawnerBase where R : Resource where N : Node
{
	protected R? Resource { get; set; }

	/// <summary>
	/// Exposes the provided resource to Godot's save system.
	/// </summary>
	[Export]
	private Resource? Value
	{
		get => Resource;
		set => SetResource(value as R);
	}

	public N? Node { get; protected set; }

	public Spawner() { }
	public Spawner(R resource) => SetResource(resource);

	protected abstract N? _Spawn(R resource, Node3D world);
	protected virtual void _Update(R resource, N node) { }
	protected virtual void _Despawn(N node)
	{
		if (node.CanQueueFree())
			node.QueueFree();
	}
	protected virtual bool _IsSubSpawner() => false;

	public sealed override R? GetResource() => Resource;

	protected sealed override void SetResource(Resource? value)
	{
		Resource?.TryDisconnectChanged(Callable.From(Update));
		value?.TryConnectChanged(Callable.From(Update));
		Resource = value as R;
		EmitChanged();
	}

	public sealed override bool HasSpawned() => Node != null;
	protected sealed override bool IsSubSpawner() => _IsSubSpawner();

	public sealed override void Spawn(Node3D world)
	{
		if (Resource is not null)
			Node = _Spawn(Resource, world);
		if (Node is not null)
		{
			Node.SetMeta(MetaName, this);
			Node.AddToGroup(GroupName);
			if (!IsSubSpawner()) Node.AddToGroup(TopLevelGroupName);
		}
		Update();
	}

	protected sealed override void Update()
	{
		if (Node is not null && Resource is not null)
			_Update(Resource, Node);
	}

	public sealed override void Despawn()
	{
		if (Resource == null) return;
		if (Node is not null)
			_Despawn(Node);
		Node = null;
	}
}

public interface ISpawnable
{
	SpawnerBase GetSpawner();
}
