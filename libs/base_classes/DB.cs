using System.Collections;
using System.Collections.Generic;
using Godot;
using Godot.Collections;

public partial class DB<[MustBeVariant] T> : GodotObject, IEnumerable<T>
{
    private Array<T> _resources = [];

    public Array<T> Resources { get => _resources; }

    [Signal]
    public delegate void RegisteredEventHandler(Variant resource);

    [Signal]
    public delegate void UnregisteredEventHandler(Variant resource);

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

    public IEnumerator<T> GetEnumerator() => Resources.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => Resources.GetEnumerator();
}

public static class DBExtensions
{
    public static T? Find<[MustBeVariant] T>(this DB<T> db, StringName id) where T : IIdentifiable<StringName> => db.Resources.WhereNotNull().Find(resource => resource.ID.Equals(id));
}