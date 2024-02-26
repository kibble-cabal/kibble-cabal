using Godot;
using Godot.Collections;
using AS;
using System.Linq;

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
            return CheckFor switch
            {
                CheckType.All => system.HasAllTags(TagsToCheck.Convert<Resource, Tag>().ToList()),
                CheckType.None => !system.HasSomeTags(TagsToCheck.Convert<Resource, Tag>().ToList()),
                CheckType.Some => system.HasSomeTags(TagsToCheck.Convert<Resource, Tag>().ToList()),
                _ => false
            };
        }

        private static AbilitySystem? GetAbilitySystem(Node node)
        {
            var nodes = node.FindChildren("", "AbilitySystem");
            if (nodes.Count > 0) return new(nodes[0]);
            return null;
        }
        
        static FilterByTags()
        {
            #if TOOLS
            JSONSchema.GeneratorDB.Register(new JSONSchema.Generator
            {
                ClassName = nameof(FilterByTags),
                Path = "res://docs/schemas/query/FilterByTags.schema.json",
                Title = "Filter Query Results By Tags"
            });
            #endif
        }
    }
}