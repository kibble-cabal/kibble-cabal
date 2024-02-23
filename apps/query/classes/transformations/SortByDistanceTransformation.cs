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
            _ => input.OrderByDescending(result => result.Distance)
        };
    }
}