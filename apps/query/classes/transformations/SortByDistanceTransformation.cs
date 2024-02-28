using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Query.Transformation
{
    public partial class SortByDistanceTransformation :
        Resource,
        ITransformation<IEnumerable<PhysicsQuery.Result>, IEnumerable<PhysicsQuery.Result>>
    {
        public enum Order
        {
            ClosestFirst,
            FurthestFirst
        }

        [Export]
        public Order SortOrder = Order.ClosestFirst;

        public IEnumerable<PhysicsQuery.Result> Transform(IEnumerable<PhysicsQuery.Result> input) => SortOrder switch
        {
            Order.ClosestFirst => input.OrderBy(result => result.Distance),
            Order.FurthestFirst => input.OrderByDescending(result => result.Distance),
            _ => throw new System.NotImplementedException()
        };
        
        static SortByDistanceTransformation()
        {
            #if TOOLS
            JSON.Schema.GeneratorDB.Register(new JSON.Schema.Generator
            {
                ClassName = nameof(SortByDistanceTransformation),
                Path = "res://docs/schemas/query/SortByDistance.schema.json",
                Title = "Sort Query Results by Distance"
            });
            #endif
        }
    }
}