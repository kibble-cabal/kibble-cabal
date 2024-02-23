
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace KibbleCabal.Apps.AI.Task
{

    public interface IBTRunSubTreesFromDB
    {
        StringName HookKey { get; set; }
        string GetName() => $"Run \"{HookKey}\" SubTrees from DB";
        IEnumerable<BTTask> InstantiateSubTrees(Node agent, Blackboard blackboard)
        {
            if (string.IsNullOrEmpty(HookKey)) return [];
            return SubTreeDB.FindByHook(HookKey)
                .Select(tree => tree.SubTree)
                .WhereNotNull()
                .Select(tree => tree.Instantiate(agent, blackboard));
        }
        static void Setup<T>(T task) where T : BTTask, IBTRunSubTreesFromDB => task
            .InstantiateSubTrees(task.Agent, task.Blackboard)
            .ForEach(task.AddChild);
    }
}