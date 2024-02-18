using Godot;
using Godot.Collections;

public partial class DB<[MustBeVariant] T> : GodotObject
{
    private Array<T> _resources = [];

    public Array<T> Resources { get => _resources; }

    [Signal]
    public delegate void RegisteredEventHandler(Resource resource);

    [Signal]
    public delegate void UnregisteredEventHandler(Resource resource);

    public void Register(T resource)
    {
        if (!Resources.Contains(resource))
        {
            Resources.Add(resource);
            EmitSignal(SignalName.Registered, [Variant.From(resource)]);
        }
    }

    public void Unregister(T resource)
    {
        if (Resources.Contains(resource))
        {
            Resources.Remove(resource);
            EmitSignal(SignalName.Unregistered, [Variant.From(resource)]);
        }
    }
}

public partial class SingletonDB<[MustBeVariant] T> : Singleton<DB<T>>
{
    public static Array<T> Resources { get => Instance.Resources; }
    public static void Register(T resource) => Instance.Register(resource);
    public static void Unregister(T resource) => Instance.Unregister(resource);
}

public partial class SingletonDB<T> : Singleton<DB<T>> where T : IIdentifiable<StringName>
{
    public static T? Find(StringName id) => Instance.Find<T>(id);
}

public static class DBExtensions
{
    public static T? Find<[MustBeVariant] T>(this DB<T> db, StringName id) where T : IIdentifiable<StringName> => db.Resources.WhereNotNull().Find(resource => resource.ID.Equals(id));
}