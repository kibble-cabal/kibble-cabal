using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KibbleCabal.Apps.Pet
{
    public partial class PetScene : PetBody3D
    {
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

        private Node? SpriteController;
        private Viewport? Viewport;
        private Camera3D? Camera;
        public AbilitySystem? AbilitySystem;
        public BTPlayer? BehaviorTree;

        public override void _Ready()
        {
            MoveStarted += OnMoveStarted;
            MoveFinished += OnMoveFinished;

            var node = GetNodeOrNull(AbilitySystemPath);
            if (node is not null)
                AbilitySystem = new(node);

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
            }
        }

        public override void _UnhandledInput(InputEvent @event)
        {
            if (@event is InputEventScreenTouch && @event.IsPressed() && ActionMenu is not null && ActionMenu.Visible)
            {
                var menuRadius = Mathf.Max(ActionMenu.Size.X, ActionMenu.Size.Y) / 2;
                if (ActionMenu.GetLocalMousePosition().DistanceTo(ActionMenu.Size / 2) > menuRadius)
                {
                    ActionMenu.Close();
                    Viewport?.SetInputAsHandled();
                }
            }
        }

        private void InstantiateSpriteController()
        {
            var animal = Resource?.GetAnimal();
            if (Resource is null || animal is null || animal.SpriteScene is null) return;
            SpriteController = animal.SpriteScene.Instantiate();
            AddChild(SpriteController);
            MoveChild(SpriteController, 0);
        }

        private void UpdateCollision()
        {
            var animal = Resource?.GetAnimal();
            if (Resource is null || animal is null) return;
            if (NavigationAgent is not null)
                NavigationAgent.Radius = animal.CollisionRadius;
            if (CollisionShape is not null && CollisionShape.Shape is SphereShape3D sphere)
                sphere.Radius = animal.CollisionRadius;
            if (InputShape is not null && InputShape.Shape is SphereShape3D inputSphere)
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

        public static void DestroyThoughtBubble(ThoughtBubble bubble)
        {
            if (!bubble.CanQueueFree()) return;
            _ = bubble.Destroy();
        }

        public async Task DestroyThoughtBubbles()
        {
            GetChildren()
                .Where(child => child.IsInGroup(ThoughtBubble.GroupName))
                .ForEach(child => DestroyThoughtBubble((ThoughtBubble)child));
            await ToSignal(GetTree().CreateTimer(0.25), Timer.SignalName.Timeout);
        }

        public async Task SpawnThoughtBubble(string text, float duration = 3, float maxWidth = -1)
        {
            await DestroyThoughtBubbles();
            var bubble = new ThoughtBubble(text, duration, maxWidth)
            {
                LocalPosition = Vector3.Zero,
                ScreenOffset = new Vector2(maxWidth / 2, -20)
            };
            AddChild(bubble);
        }

        private void OnMoveStarted()
        {
            SpriteController?.Call("start", ["walk"]);
        }

        private void OnMoveFinished()
        {
            SpriteController?.Call("start", ["default"]);
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
