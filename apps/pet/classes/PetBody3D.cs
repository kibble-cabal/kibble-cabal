using Godot;

namespace KibbleCabal.Apps.Pet
{
    public partial class PetBody3D : CharacterBody3D
    {
        [Signal]
        public delegate void MoveFinishedEventHandler();

        [Signal]
        public delegate void MoveStartedEventHandler();

        [Export]
        public NavigationAgent3D? NavigationAgent;

        [Export]
        public RayCast3D? FacingRay;

        private bool _isMoving = false;

        private float FacingRayLength;

        public bool IsMoving
        {
            get => _isMoving;
            set
            {
                switch (_isMoving, value)
                {
                    case (false, true):
                        EmitSignal(SignalName.MoveStarted);
                        break;
                    case (true, false):
                        EmitSignal(SignalName.MoveFinished);
                        break;
                }
                _isMoving = value;
            }
        }

        public override void _Ready()
        {
            if (NavigationAgent is not null)
                NavigationAgent.NavigationFinished += () => EmitSignal(SignalName.MoveFinished);
            if (FacingRay is not null)
                FacingRayLength = FacingRay.TargetPosition.Length();
        }

        public override void _PhysicsProcess(double delta)
        {
            IsMoving = !Velocity.IsZeroApprox();
            if (IsMoving)
            {
                if (FacingRay is not null)
                    FacingRay.TargetPosition = (FacingRayLength.ToVector3() * Velocity.Sign()).LimitLength(FacingRayLength);
                MoveAndSlide();
            }
        }
    }
}