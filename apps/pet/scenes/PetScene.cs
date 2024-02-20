using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace KibbleCabal.Apps.Pet
{
    public partial class PetScene : PetBody3D
    {
        public static readonly GDScript ThoughtBubbleScript = GD.Load<GDScript>("res://apps/ui/classes/nodes/thought_bubble.gd");
        public static readonly StringName ThoughtBubbleGroup = "thought_bubble";

        [Export]
        public RPet? Resource;

        [Export]
        private Vector3 StartPosition;

        [Export]
        private Node? AbilitySystem;

        [Export]
        private ContextActionMenu? ActionMenu;

        [Export]
        private Node? BehaviorTree;

        [Export]
        private CollisionShape3D? CollisionShape;

        [Export]
        private Area3D? InputArea;

        [Export]
        private CollisionShape3D? InputShape;

        private Node? SpriteController;
        private Viewport? Viewport;
        private Camera3D? Camera;

        public override void _Ready()
        {
            MoveStarted += OnMoveStarted;
            MoveFinished += OnMoveFinished;

            Viewport = GetViewport();
            Camera = Viewport?.GetCamera3D();
            StartPosition = GlobalPosition;

            GlobalPosition = Resource?.Position ?? Vector3.Zero;

            InstantiateSpriteController();
            UpdateCollision();
            UpdateSpeed();
            // TODO: Ability state
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

        public Vector3 GetRandomTarget() => new(GD.RandRange(-2, 2), 0, GD.RandRange(-2, 2));

        public void DestroyThoughtBubble(Label bubble)
        {
            if (!bubble.CanQueueFree()) return;
            bubble.Call("destroy");
        }

        public async Task DestroyThoughtBubbles()
        {
            GetChildren()
                .Where(child => child.IsInGroup(ThoughtBubbleGroup))
                .ForEach(child => DestroyThoughtBubble((Label)child));
            await ToSignal(GetTree().CreateTimer(0.25), Timer.SignalName.Timeout);
        }

        public async Task SpawnThoughtBubble(string text, float duration = 3, float maxWidth = -1)
        {
            await DestroyThoughtBubbles();
            var bubble = ThoughtBubbleScript.New<Label>();
            bubble?.Call("initialize", [text, duration, maxWidth]);
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
                ActionMenu?.Open(new RPetContextAction.Context { Pet = Resource, Node = this });
        }
    }
}
