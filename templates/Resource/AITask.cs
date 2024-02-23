using Godot;

namespace KibbleCabal.Apps.AI.Task
{
    /// <summary>
    /// This is the description of the task.
    /// </summary>
    [Tool]
    public partial class _CLASS_ : BTAction
    {
        public override string _GenerateName() => "";

        public override string[] _GetConfigurationWarnings()
        {
            return [];
        }

        public override void _Enter() { }

        public override void _Exit() { }

        public override Status _Tick(double delta)
        {
            return Status.Success;
        }
    }
}