using Godot;
using KibbleCabal.Apps.Pet;

namespace KibbleCabal.Apps.AI.Task
{
    [Tool]
    public partial class BTWhileInstructionsEmpty : BTDecorator
    {
        [Export]
        public Status OnInstructionFound = Status.Failure;

        public override string _GenerateName() => "While this pet has no instructions...";

        public override Status _Tick(double delta)
        {
            if (GetChildCount() == 0) return Status.Success;
            var pet = GetPet();
            if (pet is null) return Status.Failure;

            // Execute child if there are no instructions, or if this node is playing within an instruction.
            if (pet.Instructions.Count == 0 || Blackboard.IsInstruction())
                return GetChild(0).Execute(delta);
            else
            {
                GetChild(0).Abort();
                return OnInstructionFound;
            }
        }

        protected RPet? GetPet() => (Agent as PetScene)?.Resource;
    }
}
