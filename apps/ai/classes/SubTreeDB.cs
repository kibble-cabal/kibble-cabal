using System.Collections.Generic;
using System.Linq;
using Godot;

public sealed partial class SubTreeDB : SingletonDB<RSubTree>
{
    public static IEnumerable<RSubTree> FindByHook(StringName hook) => Resources.Where(subtree => subtree.Hook == hook);
}