using System.Collections.Generic;
using Godot;

using Subresources = Godot.Collections.Dictionary<Godot.Variant, Godot.Resource>;

[Tool]
[GlobalClass]
public abstract partial class ExtensibleResource : Resource
{
    private Subresources _subResources = [];

    [Export]
    public Subresources SubResources
    {
        get => _subResources;
        private set => SetSubResources(value);
    }

    [Signal]
    public delegate void SubResourceChangedEventHandler(Resource resource);

    [Signal]
    public delegate void SubResourcesChangedEventHandler();

    protected R? GetSubResource<R, [MustBeVariant] K>(K key) where R : Resource => GetSubResource<R>(Variant.From(key));
    protected R? GetSubResource<R>(Variant key) where R : Resource => (R?)_subResources.Get(key);

    protected R ExpectSubResource<R, [MustBeVariant] K>(K key) where R : Resource, new() => ExpectSubResource<R>(Variant.From(key));
    protected R ExpectSubResource<R>(Variant key) where R : Resource, new()
    {
        if (GetSubResource<R>(key) is R resource) return resource;
        SetSubResource(key, new R());
        return (R)_subResources[key];
    }

    public void SetSubResources(Subresources value)
    {
        DisconnectAllSubResources();
        _subResources = value;
        ConnectAllSubResources();
        EmitSignal(SignalName.SubResourcesChanged);
    }

    public void SetSubResource<R, [MustBeVariant] K>(K key, R? resource) where R : Resource => SetSubResource<R>(Variant.From(key), resource);
    public void SetSubResource<R>(Variant key, R? resource) where R : Resource
    {
        RemoveSubresource(key);
        if (resource is Resource value)
        {
            _subResources[key] = value;
            ConnectSubresource(value);
        }
    }

    public void RemoveSubresource(Variant key)
    {
        if (_subResources.TryGetValue(key, out Resource? currentValue))
            DisconnectSubresource(currentValue);
        _subResources.Remove(key);
    }

    protected virtual IEnumerable<Resource> _GetAllSubResources() => _subResources.Values;
    protected virtual bool _ShouldEmitChanged(Resource resource) => true;
    protected void DisconnectAllSubResources() => _GetAllSubResources().ForEach(DisconnectSubresource);
    protected void ConnectAllSubResources() => _GetAllSubResources().ForEach(ConnectSubresource);

    protected void DisconnectSubresource(Resource resource) => resource.DisconnectAllFromTarget(signal: Resource.SignalName.Changed, target: this);
    protected void ConnectSubresource(Resource resource) => resource.TryConnect(Resource.SignalName.Changed, Callable.From(() => OnResourceChanged(resource)));

    private void OnResourceChanged(Resource resource)
    {
        EmitSignal(SignalName.SubResourceChanged, [Variant.From(resource)]);
        if (_ShouldEmitChanged(resource))
        {
            EmitSignal(SignalName.SubResourcesChanged);
            EmitChanged();
        }
    }
}