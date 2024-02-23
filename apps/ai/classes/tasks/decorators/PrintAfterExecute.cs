using Godot;

namespace KibbleCabal.Apps.AI.Task.Decorator
{
    [Tool]
    public sealed partial class PrintAfterExecute : BTPrint
    {
        public override string _GenerateName() => $"Print \"{Text}\" after...";
        public override Status _Tick(double delta)
        {
            if (GetChildCountExcludingComments() == 0) return Status.Failure;
            var child = GetChild(0);
            var childStatus = child.Execute(delta);
            Print();
            return childStatus;
        }
    }
}