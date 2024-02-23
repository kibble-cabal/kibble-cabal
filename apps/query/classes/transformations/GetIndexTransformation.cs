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
    }
}