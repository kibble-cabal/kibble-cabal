using System.Collections.Generic;
using System.Linq;
using Godot;

public sealed partial class ContextActionDB : SingletonDB<RContextAction>
{
    public static IEnumerable<RContextAction> FindByMenu(StringName menu) => Instance.Resources.Where(resource => resource.GetMenuIdentifiers().Contains(menu));
}