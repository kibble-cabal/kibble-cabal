
using Godot;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using GD = Godot.Collections;

public struct MaterialMap(GD.Dictionary<StringName, StringName> map) : IEnumerable<KeyValuePair<StringName, StringName>>, IDictionary<StringName, StringName>, ICollection<KeyValuePair<StringName, StringName>>
{
    public GD.Dictionary<StringName, StringName> Map { get; set; } = map;

    public readonly StringName this[StringName key] { get => Map[key]; set => Map[key] = value; }
    public readonly ICollection<StringName> Keys => Map.Keys;
    public readonly ICollection<StringName> Values => Map.Values;
    public readonly int Count => Map.Count;
    public readonly bool IsReadOnly => Map.IsReadOnly;

    public static implicit operator GD.Dictionary<StringName, StringName>(MaterialMap map) => map.Map;
    public static implicit operator Variant(MaterialMap map) => map.Map;
    public static implicit operator MaterialMap(Variant map) => new(map.As<GD.Dictionary<StringName, StringName>>());
    public static implicit operator MaterialMap(GD.Dictionary<StringName, StringName> map) => new(map);

    public readonly void Add(StringName key, StringName value) => Map.Add(key, value);
    public readonly void Add(KeyValuePair<StringName, StringName> item) => Map.Add(item.Key, item.Value);
    public readonly void Clear() => Map.Clear();
    public readonly bool Contains(KeyValuePair<StringName, StringName> item) => Map.Contains(item);
    public readonly bool ContainsKey(StringName key) => Map.ContainsKey(key);
    public readonly void CopyTo(KeyValuePair<StringName, StringName>[] array, int arrayIndex) { throw new System.NotImplementedException(); }
    public readonly bool Remove(StringName key) => Map.Remove(key);
    public readonly bool Remove(KeyValuePair<StringName, StringName> item) => Map.Remove(item.Key);
    public readonly bool TryGetValue(StringName key, [MaybeNullWhen(false)] out StringName value) => Map.TryGetValue(key, out value);
    public readonly IEnumerator<KeyValuePair<StringName, StringName>> GetEnumerator() => Map.GetEnumerator();
    readonly IEnumerator IEnumerable.GetEnumerator() => Map.GetEnumerator();
}
