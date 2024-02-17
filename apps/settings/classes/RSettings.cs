using Godot;
using Godot.Collections;

[GlobalClass]
public sealed partial class RSettings : ExtensibleResource
{
    private Dictionary<StringName, Variant> _settings = [];

    [Export]
    public Dictionary<StringName, Variant> Settings
    {
        get => _settings;
        private set => this.Set(ref _settings, value);
    }

    public void Change(StringName key, Variant value)
    {
        if (_settings.ContainsKey(key)) _settings[key] = value;
        else _settings.Add(key, value);
        EmitChanged();
    }

    public T Get<[MustBeVariant] T>(StringName key) => _settings.Get(key).As<T>();
}