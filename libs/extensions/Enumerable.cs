using System.Linq;
using System.Collections.Generic;
using System;
using Godot;

using GDC = Godot.Collections;
using Godot.Collections;

#nullable enable

public static class EnumerableExtensions
{
    public static void ForEach<T>(this IEnumerable<T> value, Action<T> action)
    {
        foreach (var element in value) action(element);
    }

    public static void ForEach<T>(this IEnumerable<T> value, Action<T, int> action)
    {
        int i = 0;
        foreach (var element in value)
        {
            action(element, i);
            i += 1;
        }
    }

    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T?> value) => value.Where(value => value != null).Select<T?, T>(value => value!);

    public static T? Find<T>(this IEnumerable<T> value, Func<T, bool> predicate)
    {
        foreach (var element in value)
            if (predicate(element)) return element;
        return default;
    }

    public static GDC.Array<T> ToGodotArray<[MustBeVariant] T>(this IEnumerable<T> array) => new(array);
    public static GDC.Array ToGodotArray(this IEnumerable<Variant> array) => new(array);

    public static GDC.Dictionary<K, V> ToGodotDictionary<[MustBeVariant] K, [MustBeVariant] V>(this IEnumerable<(K Key, V Value)> pairs)
    {
        var dict = new GDC.Dictionary<K, V>();
        foreach (var (key, value) in pairs) dict[key] = value;
        return dict;
    }

    public static V? Get<K, V>(this IDictionary<K, V> dict, K key) => dict.Get<K, V>(key, default!);

    public static V? Get<K, V>(this IDictionary<K, V> dict, K key, V defaultValue)
    {
        if (dict.ContainsKey(key)) return dict[key];
        return defaultValue;
    }

    public static T? Pop<[MustBeVariant] T>(this GDC.Array<T> array)
    {
        if (array.Count > 0)
        {
            var item = array[^1];
            array.RemoveAt(array.Count - 1);
            return item;
        }
        return default;
    }

    public static bool Has<T>(this IEnumerable<T> values, int index) => index >= 0 && index < values.Count();

    public static void AddDistinct<T>(this List<T> values, IEnumerable<T> other) => values.AddRange(other.Except(values));
    public static void AddDistinct<[MustBeVariant] T>(this Array<T> values, IEnumerable<T> other) => values.AddRange(other.Except(values));

    public static string JoinToString<T>(this IEnumerable<T> values) => string.Join("", values.Select(v => v?.ToString()));
}