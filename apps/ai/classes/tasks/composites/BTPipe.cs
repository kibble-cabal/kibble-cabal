using Godot;

namespace KibbleCabal.Apps.AI.Task
{
    /// <summary>
    /// Executes all children in order, regardless of whether they succeed or fail.
    /// This means that this task ALWAYS succeeds.
    /// </summary>
    [Tool]
    public partial class BTPipe : BTComposite
    {
        protected int LastRunningIndex = 0;

        public override string _GenerateName() => "Pipe";

        public override Status _Tick(double delta)
        {
            for (int i = LastRunningIndex; i < GetChildCount(); i++)
            {
                if (GetChild(i).Execute(delta) == Status.Running)
                {
                    LastRunningIndex = i;
                    return Status.Running;
                }
            }
            return Status.Success;
        }
    }
}
