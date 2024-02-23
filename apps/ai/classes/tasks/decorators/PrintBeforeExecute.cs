using Godot;

namespace KibbleCabal.Apps.AI.Task.Decorator
{
    [Tool]
    public sealed partial class PrintBeforeExecute : BTPrint
    {
        public override string _GenerateName() => $"Print \"{Text}\", then...";
        public override Status _Tick(double delta)
        {
            if (GetChildCountExcludingComments() == 0) return Status.Failure;
            Print();
            return GetChild(0).Execute(delta);
        }
    }
}