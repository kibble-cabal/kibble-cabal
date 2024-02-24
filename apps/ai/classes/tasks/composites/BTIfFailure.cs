using Godot;

namespace KibbleCabal.Apps.AI.Task
{
    /// <summary>
    /// Executes the second child only if the first child fails. Otherwise, executes third child.
    /// </summary>
    [Tool]
    public partial class BTIfFailure : BTSelector
    {
        private Status ConditionStatus = Status.Fresh;

        public override string _GenerateName() => "If 1st Child Fails, Execute 2nd";

        public override Status _Tick(double delta)
        {
            if (GetChildCountExcludingComments() < 2) return Status.Failure;

            if (ConditionStatus != Status.Success && ConditionStatus != Status.Failure)
                ConditionStatus = GetChild(0).Execute(delta);

            return ConditionStatus switch
            {
                Status.Running => Status.Running,
                Status.Failure => GetChild(1).Execute(delta),
                Status.Success when GetChildCountExcludingComments() > 2 => GetChild(2).Execute(delta),
                _ => Status.Failure,
            };
        }
    }
}