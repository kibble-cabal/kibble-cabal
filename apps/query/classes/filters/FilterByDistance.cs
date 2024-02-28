using Godot;

namespace Query.Filter
{
    [GlobalClass]
    public partial class FilterByDistance : Resource, IFilter<PhysicsQuery.Result>
    {
        public enum Op
        {
            LessThan,
            LessThanOrEqualTo,
            EqualTo,
            GreaterThan,
            GreaterThanOrEqualTo
        }

        [Export]
        public Op Operator = Op.LessThan;

        [Export]
        public float Distance = 0;

        public bool Filter(PhysicsQuery.Result input) => Operator switch
        {
            Op.LessThan => input.Distance < Distance,
            Op.LessThanOrEqualTo => input.Distance <= Distance,
            Op.GreaterThan => input.Distance > Distance,
            Op.GreaterThanOrEqualTo => input.Distance >= Distance,
            Op.EqualTo => input.Distance.IsEqualApprox(Distance),
            _ => false,
        };
        
        static FilterByDistance()
        {
            #if TOOLS
            JSON.Schema.GeneratorDB.Register(new JSON.Schema.Generator
            {
                ClassName = nameof(FilterByDistance),
                Path = "res://docs/schemas/query/FilterByDistance.schema.json",
                Title = "Filter Query Results By Distance"
            });
            #endif
        }
    }
}