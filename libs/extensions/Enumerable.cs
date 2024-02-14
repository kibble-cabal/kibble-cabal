using System.Linq;
using System.Collections.Generic;
using System;
using Godot;

#nullable enable

public static class EnumerableExtensions
{
    public static IEnumerable<T> WhereNotNull<T>(this IEnumerable<T> value) => value.Where(value => value != null);

    public static T? Find<T>(this IEnumerable<T> value, Func<T, bool> predicate)
    {
        foreach (var element in value)
            if (predicate(element)) return element;
        return default;
    }

    public static Godot.Collections.Array<T> ToGodotArray<[MustBeVariant] T>(this IEnumerable<T> array) => new Godot.Collections.Array<T>(array);
    public static Godot.Collections.Array ToGodotArray(this IEnumerable<Variant> array) => new Godot.Collections.Array(array);
}