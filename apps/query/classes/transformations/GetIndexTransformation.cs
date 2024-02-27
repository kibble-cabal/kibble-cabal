using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Query.Transformation
{
    public partial class GetIndexTransformation<In> : 
        Resource, 
        ITransformation<IEnumerable<In>, In>
    {
        [Export]
        public int Index;

        public In? Transform(IEnumerable<In?> input) => input.ElementAtOrDefault(Index);
    }
}