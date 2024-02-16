using System;

public abstract class Singleton<T> where T : new()
{
    public static readonly Lazy<T> lazy = new(() => new T());
    public static T Instance { get { return lazy.Value; } }
}