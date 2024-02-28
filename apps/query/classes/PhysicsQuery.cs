using System.Collections.Generic;
using Godot;

namespace Query
{
    [GlobalClass]
    public partial class PhysicsQuery : Query<Node3D, PhysicsQuery.Result>
    {
        public struct Result
        {
            public CollisionObject3D Collider;
            public Vector3 CollisionPoint;
            public float Distance;

            public override string ToString() => Json.Stringify(new Godot.Collections.Dictionary
            {
                { "Collider", Collider },
                { "CollisionPoint", CollisionPoint },
                { "Distance", Distance.ToPrecisionString() }
            }, "  ", false);
        }

        [Export]
        public Shape3D? Region;

        [Export(PropertyHint.Layers3DPhysics)]
        public uint CollisionMask = 0;

        [Export]
        public bool DetectBodies = true;

        [Export]
        public bool DetectAreas = false;

        protected override IEnumerable<Result> Search(Node3D caller)
        {
            var cast = MakeShapeCast();
            caller.AddChild(cast);
            cast.ForceShapecastUpdate();
            List<Result> colliders = [];
            for (int i = 0; i < cast.GetCollisionCount(); i++)
            {
                var point = cast.GetCollisionPoint(i);
                colliders.Add(new Result
                {
                    Collider = (cast.GetCollider(i) as CollisionObject3D)!,
                    CollisionPoint = point,
                    Distance = point.DistanceTo(caller.GlobalPosition).Abs()
                });
            }
            caller.RemoveChild(cast);
            cast.QueueFree();
            return colliders;
        }

        private ShapeCast3D MakeShapeCast() => new()
        {
            Shape = Region,
            TargetPosition = Vector3.Zero,
            CollisionMask = CollisionMask,
            CollideWithBodies = DetectBodies,
            CollideWithAreas = DetectAreas,
        };
        
        static PhysicsQuery()
        {
            #if TOOLS
            JSON.Schema.GeneratorDB.Register(new JSON.Schema.Generator
            {
                ClassName = nameof(PhysicsQuery),
                Path = "res://docs/schemas/query/PhysicsQuery.schema.json",
                Title = "Physics Query"
            });
            #endif
        }
    }
}