using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Query.Transformation
{
    [GlobalClass]
    public partial class GetCollisionPointTransformation :
        Resource,
        ITransformation<PhysicsQuery.Result, Vector3>,
        ITransformation<IEnumerable<PhysicsQuery.Result>, IEnumerable<Vector3>>
    {
        public Vector3 Transform(PhysicsQuery.Result input) => input.CollisionPoint;
        public IEnumerable<Vector3> Transform(IEnumerable<PhysicsQuery.Result> input) => input.Select(result => result.CollisionPoint);
        
        static GetCollisionPointTransformation()
        {
            #if TOOLS
            JSON.Schema.GeneratorDB.Register(new JSON.Schema.Generator
            {
                ClassName = nameof(GetCollisionPointTransformation),
                Path = "res://docs/schemas/query/GetCollisionPoint.schema.json",
                Title = "Get Collision Point of Query Result(s)"
            });
            #endif
        }
    }
}