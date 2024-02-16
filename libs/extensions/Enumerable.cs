using System.Linq;
using System.Collections.Generic;
using System;
using Godot;

#nullable enable

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> value, Action<T> action)
    {
        foreach (var element in value) action(element);
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> value) => value.Where(value => value != null).Select<T?, T>(value => value!);

    public static T? Find<T>(this IEnumerable<T> value, Func<T, bool> predicate)
    {
        foreach (var element in value)
            if (predicate(element)) return element;
        return default;
    }

    public static Godot.Collections.Array<T> ToGodotArray<[MustBeVariant] T>(this IEnumerable<T> array) => new Godot.Collections.Array<T>(array);
    public static Godot.Collections.Array ToGodotArray(this IEnumerable<Variant> array) => new Godot.Collections.Array(array);

    public static V? Get<K, V>(this IDictionary<K, V> dict, K key) => dict.Get<K, V>(key, default!);

    public static V? Get<K, V>(this IDictionary<K, V> dict, K key, V defaultValue)
    {
        if (dict.ContainsKey(key)) return dict[key];
        return defaultValue;
    }

}