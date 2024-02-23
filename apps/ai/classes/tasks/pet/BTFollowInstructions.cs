using System.Linq;
using Godot;
using KibbleCabal.Apps.Pet;

namespace KibbleCabal.Apps.AI.Task
{
	[Tool]
	public partial class BTFollowInstructions : BTAction
	{
		public override string _GenerateName() => "Follow Instructions";

		public override void _Enter()
		{
			var pet = GetPet();
			if (pet is not null)
			{
				Blackboard.SetVar(BTContext.VarName.IsInstruction, true);
				pet.Instructions
					.Select(tree => tree.Instantiate(Agent, Blackboard))
					.ForEach(AddChild);
			}
			Blackboard.SetVar(BTContext.VarName.IsInstruction, true);
		}

		public override void _Exit()
		{
			Blackboard.SetVar(BTContext.VarName.IsInstruction, false);
		}

		public override Status _Tick(double delta)
		{
			var pet = GetPet();
			if (pet is null) return Status.Failure;
			if (pet.Instructions.Count == 0) return Status.Success;
			var index = pet.Instructions.Count - 1;
			var childStatus = GetChild(index).Execute(delta);
			if (childStatus == Status.Success || childStatus == Status.Failure)
				pet.Instructions.Pop();
			return Status.Running;
		}

		protected RPet? GetPet() => (Agent as PetScene)?.Resource;
	}
}
