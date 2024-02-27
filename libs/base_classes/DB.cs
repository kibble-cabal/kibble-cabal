using Godot;
using System;
using System.Collections.Generic;

public class DB<T> where T: class
{
    public List<T> Resources { get; } = [];

    public event Action<T>? Registered;
    public event Action<T>? Unregistered;

    public void Register(T resource)
    {
        if (!Resources.Contains(resource))
        {
            Resources.Add(resource);
            Registered?.Invoke(resource);
        }
    }

    public void Unregister(T resource)
    {
        if (Resources.Contains(resource))
        {
            Resources.Remove(resource);
            Unregistered?.Invoke(resource);
        }
    }
}

public class SingletonDB<T> : Singleton<DB<T>> where T: class
{
    public static List<T> Resources => Instance.Resources;
    public static void Register(T resource) => Instance.Register(resource);
    public static void Unregister(T resource) => Instance.Unregister(resource);
}

public static class DBExtensions
{
    public static T? Find<T>(this DB<T> db, StringName id) where T : class, IIdentifiable<StringName> => db.Resources.WhereNotNull().Find(resource => resource.ID.Equals(id));
    public static T? Find<T>(this DB<T> db, string id) where T : class, IIdentifiable<string> => db.Resources.WhereNotNull().Find(resource => resource.ID.Equals(id));
}