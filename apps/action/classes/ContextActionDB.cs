using System.Collections.Generic;
using System.Linq;
using Godot;

public sealed partial class ContextActionDB : SingletonDB<IContextAction>
{
    public static IEnumerable<IContextAction> FindByMenu(StringName menu) => Instance.Resources.Where(resource => resource.GetMenuIdentifiers().Contains(menu));
}