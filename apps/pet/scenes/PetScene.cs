using Godot;
using System.Linq;
using AS;

namespace KibbleCabal.Apps.Pet
{
    public partial class PetScene : PetBody3D
    {
        public ThoughtBubbleComponent ThoughtBubbleComponent;
        
        [Export]
        public RPet? Resource;

        [Export]
        private Vector3 StartPosition;

        [Export]
        private NodePath? AbilitySystemPath;

        [Export]
        private NodePath? BehaviorTreePath;

        [Export]
        private ContextActionMenu? ActionMenu;

        [Export]
        private CollisionShape3D? CollisionShape;

        [Export]
        private Area3D? InputArea;

        [Export]
        private CollisionShape3D? InputShape;

        private SpriteController? SpriteController;
        private Viewport? Viewport;
        private Camera3D? Camera;
        public AbilitySystem? AbilitySystem;
        public BTPlayer? BehaviorTree;

        public PetScene()
        {
            ThoughtBubbleComponent = new ThoughtBubbleComponent(this);
        }

        public override void _Ready()
        {
            MoveStarted += OnMoveStarted;
            MoveFinished += OnMoveFinished;

            var node = GetNodeOrNull(AbilitySystemPath);
            if (node is not null)
                AbilitySystem = new AbilitySystem(node);

            BehaviorTree = GetNodeOrNull<BTPlayer>(BehaviorTreePath);

            Viewport = GetViewport();
            Camera = Viewport?.GetCamera3D();
            StartPosition = GlobalPosition;

            GlobalPosition = Resource?.Position ?? Vector3.Zero;

            InstantiateSpriteController();
            UpdateCollision();
            UpdateSpeed();

            if (Resource is not null && AbilitySystem is not null)
            {
                Resource.AbilitySystemState.Abilities.AddDistinct(NeedsConfig.Instance.FulfillNeeds);
                Resource.AbilitySystemState.AddAttributes(NeedsConfig.Instance.Needs.Select(AttributeDB.Find).WhereNotNull());
                Resource.AbilitySystemState.MergeInto(AbilitySystem);
                PersonalityConfig.Instance.RandomizePersonality(AbilitySystem);
            }
            
            SaveSubSystem.Instance.BeforeSaved += OnBeforeSave;
        }
        
        private void OnBeforeSave()
        {
            // Update ability system state
            if (AbilitySystem is not null) Resource?.AbilitySystemState.MergeWith(AbilitySystem);
        }

        private void InstantiateSpriteController()
        {
            SpriteController = Resource?.GetAnimal()?.SpriteScene?.Instantiate<SpriteController>();
            if (SpriteController is not null)
            {
                AddChild(SpriteController);
                MoveChild(SpriteController, 0);
            }
        }

        private void UpdateCollision()
        {
            var animal = Resource?.GetAnimal();
            if (Resource is null || animal is null) return;
            if (NavigationAgent is not null)
                NavigationAgent.Radius = animal.CollisionRadius;
            if (CollisionShape?.Shape is SphereShape3D sphere)
                sphere.Radius = animal.CollisionRadius;
            if (InputShape?.Shape is SphereShape3D inputSphere)
                inputSphere.Radius = animal.CollisionRadius * 1.5f;
            if (FacingRay is not null)
                FacingRay.TargetPosition = new Vector3(animal.CollisionRadius * 1.5f, 0, 0);
        }

        private void UpdateSpeed()
        {
            var animal = Resource?.GetAnimal();
            if (Resource is null || animal is null || NavigationAgent is null) return;
            NavigationAgent.MaxSpeed = animal.Speed * 2;
        }

        public static Vector3 GetRandomTarget() => new(GD.RandRange(-2, 2), 0, GD.RandRange(-2, 2));

        private void OnMoveStarted()
        {
            SpriteController?.Start(RPet.AnimationNames.Walk);
        }

        private void OnMoveFinished()
        {
            SpriteController?.Start(RPet.AnimationNames.Default);
            if (Resource is not null)
                Resource.Position = GlobalPosition;
        }

        public void OnAreaInputEvent(Camera3D camera, InputEvent @event, Vector3 position, Vector3 normal, int shapeIndex)
        {
            if (Resource is not null && @event is InputEventScreenTouch && @event.IsPressed())
                ActionMenu?.Open(new IPetContextAction.Context { Pet = Resource, Node = this });
        }
    }
}
