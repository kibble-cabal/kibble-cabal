using System.Collections.Generic;
using Godot;

using Subresources = Godot.Collections.Dictionary<Godot.Variant, Godot.Resource>;

[GlobalClass]
public abstract partial class ExtensibleResource : Resource
{
    private Subresources _subresources = [];

    [Export]
    public Subresources Subresources
    {
        get => _subresources;
        private set => SetSubresources(value);
    }

    [Signal]
    public delegate void SubresourceChangedEventHandler(Resource resource);

    [Signal]
    public delegate void SubresourcesChangedEventHandler();

    public Resource? GetSubresource(Variant key) => _subresources.GetValueOrDefault(key);

    public void SetSubresources(Subresources value)
    {
        DisconnectAllSubresources();
        _subresources = value;
        ConnectAllSubresources();
        EmitSignal(SignalName.SubresourcesChanged);
    }

    public void SetSubresource(Variant key, Resource? resource)
    {
        RemoveSubresource(key);
        if (resource is Resource value)
        {
            _subresources[key] = value;
            ConnectSubresource(value);
        }
    }

    public void RemoveSubresource(Variant key)
    {
        if (_subresources.TryGetValue(key, out Resource? currentValue))
            DisconnectSubresource(currentValue);
        _subresources.Remove(key);
    }

    protected virtual IEnumerable<Resource> _GetAllSubresources() => _subresources.Values;
    protected virtual bool _ShouldEmitChanged(Resource resource) => true;
    protected void DisconnectAllSubresources() => _GetAllSubresources().ForEach(DisconnectSubresource);
    protected void ConnectAllSubresources() => _GetAllSubresources().ForEach(ConnectSubresource);

    private void DisconnectSubresource(Resource resource) => resource.DisconnectAllFromTarget(signal: Resource.SignalName.Changed, target: this);
    private void ConnectSubresource(Resource resource) => resource.TryConnect(Resource.SignalName.Changed, Callable.From(() => OnResourceChanged(resource)));

    private void OnResourceChanged(Resource resource)
    {
        EmitSignal(SignalName.SubresourceChanged, [Variant.From(resource)]);
        if (_ShouldEmitChanged(resource))
        {
            EmitSignal(SignalName.SubresourcesChanged);
            EmitChanged();
        }
    }
}