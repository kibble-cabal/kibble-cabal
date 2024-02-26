using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Query.Transformation
{
    [GlobalClass]
    public partial class GetIndexTransformation : Resource, ITransformation<IEnumerable<Variant>, Variant?>
    {
        [Export]
        public int Index = 0;

        public Variant? Transform(IEnumerable<Variant> input) => input.ElementAtOrDefault(Index);
        
        static GetIndexTransformation()
        {
            #if TOOLS
            JSONSchema.GeneratorDB.Register(new JSONSchema.Generator
            {
                ClassName = nameof(GetIndexTransformation),
                Path = "res://docs/schemas/query/GetIndex.schema.json",
                Title = "Get Query Result At Index"
            });
            #endif
        }
    }
}