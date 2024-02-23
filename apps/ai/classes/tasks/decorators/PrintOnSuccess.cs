using Godot;

namespace KibbleCabal.Apps.AI.Task.Decorator
{
    /// <summary>
    /// Executes child. If it succeeds, prints given string with format params. Returns child's status.
    /// </summary>
    [Tool]
    public sealed partial class PrintOnSuccess : BTPrint
    {
        public override string _GenerateName() => $"If success, print \"{Text}\"";
        public override Status _Tick(double delta)
        {
            if (GetChildCountExcludingComments() == 0) return Status.Failure;
            var childStatus = GetChild(0).Execute(delta);
            if (childStatus == Status.Success)
                Print();
            return childStatus;
        }
    }
}