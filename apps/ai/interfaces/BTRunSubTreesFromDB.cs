using System.Linq;
using Godot;

namespace KibbleCabal.Apps.AI.Task
{
    public interface IBTRunSubTreesFromDB
    {
        StringName HookKey { get; set; }
    }

    public static class BTRunSubTreesFromDBExtension
    {
        public static string GetName<T>(this T task) where T : IBTRunSubTreesFromDB => $"Run \"{task.HookKey}\" SubTrees from DB";
        public static void Setup<T>(this T task) where T : BTTask, IBTRunSubTreesFromDB
        {
            if (Engine.IsEditorHint()) return;
            if (string.IsNullOrEmpty(task.HookKey)) return;
            GD.Print("Null subtrees: ",  SubTreeDB
                .FindByHook(task.HookKey)
                .Count(tree => tree.SubTree == null));
            SubTreeDB.FindByHook(task.HookKey)
                .Select(tree => tree.SubTree)
                .WhereNotNull()
                .Select(tree => tree.Instantiate(task.Agent, task.Blackboard))
                .ForEach(task.AddChild);
        }
    }
}