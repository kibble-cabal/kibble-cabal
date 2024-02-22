using Godot;
using System;
using System.Collections.Generic;

public partial class DB<T>
{
    private readonly List<T> _resources = [];

    public List<T> Resources { get => _resources; }

    public event EventHandler<T>? Registered;
    public event EventHandler<T>? Unregistered;

    public void Register(T resource)
    {
        if (!Resources.Contains(resource))
        {
            Resources.Add(resource);
            Registered?.Invoke(this, resource);
        }
    }

    public void Unregister(T resource)
    {
        if (Resources.Contains(resource))
        {
            Resources.Remove(resource);
            Unregistered?.Invoke(this, resource);
        }
    }
}

public partial class SingletonDB<T> : Singleton<DB<T>>
{
    public static List<T> Resources { get => Instance.Resources; }
    public static void Register(T resource) => Instance.Register(resource);
    public static void Unregister(T resource) => Instance.Unregister(resource);
}

public static class DBExtensions
{
    public static T? Find<T>(this DB<T> db, StringName id) where T : IIdentifiable<StringName> => db.Resources.WhereNotNull().Find(resource => resource.ID.Equals(id));
}