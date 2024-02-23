using Godot;
using Godot.Collections;

namespace Query.Filter
{
    [GlobalClass]
    public partial class FilterByTags : Resource, IFilter<PhysicsQuery.Result>
    {
        public enum CheckType
        {
            All,
            None,
            Some
        }

        [Export]
        public Array<Resource> TagsToCheck = [];

        [Export]
        public CheckType CheckFor = CheckType.Some;

        [Export]
        public float Distance = 0;

        public bool Filter(PhysicsQuery.Result input)
        {
            var system = GetAbilitySystem(input.Collider);
            if (system is null) return false;
            Godot.Collections.Array tags = (Godot.Collections.Array)TagsToCheck;
            return CheckFor switch
            {
                CheckType.All => system.HasAllTags(tags),
                CheckType.None => !system.HasSomeTags(tags),
                CheckType.Some => system.HasSomeTags(tags),
                _ => false
            };
        }

        private static AbilitySystem? GetAbilitySystem(Node node)
        {
            var nodes = node.FindChildren("", "AbilitySystem");
            if (nodes.Count > 0) return new(nodes[0]);
            return null;
        }
    }
}