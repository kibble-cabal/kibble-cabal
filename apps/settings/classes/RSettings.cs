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

    public void Change<[MustBeVariant] T>(StringName key, T value)
    {
        if (_settings.ContainsKey(key)) _settings[key] = Variant.From(value);
        else _settings.Add(key, Variant.From(value));
        EmitChanged();
    }

    public T Get<[MustBeVariant] T>(StringName key) => _settings.Get(key, new Variant()).As<T>();
    
    static RSettings()
    {
        #if TOOLS
        JSON.Schema.GeneratorDB.Register(new JSON.Schema.Generator
        {
            ClassName = nameof(RSettings),
            Path = "res://docs/schemas/Settings.schema.json",
            Title = "Settings"
        });
        #endif
    }
}