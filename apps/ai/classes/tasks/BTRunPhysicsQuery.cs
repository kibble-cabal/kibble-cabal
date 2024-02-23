using Godot;
using Query;

namespace KibbleCabal.Apps.AI.Task
{
    /// <summary>
    /// Runs a query.
    /// </summary>
    [Tool]
    public partial class BTRunPhysicsQuery : BTAction
    {
        private PhysicsQuery? _query;
        private string _resultVar = "";


        [Export]
        public PhysicsQuery? Query
        {
            get => _query;
            set => this.Set(ref _query, value);
        }

        [Export]
        public string ResultVar
        {
            get => _resultVar;
            set => this.Set(ref _resultVar, value);
        }

        public override string _GenerateName()
        {
            var name = (!(Query?.ResourceName.IsEmpty() ?? true) ? Query?.ResourceName : Query?.ResourcePath) ?? "???";
            var resultString = string.IsNullOrEmpty(ResultVar) ? "" : $", set Blackboard.{ResultVar} to result";
            return $"Run Query \"{name}\"{resultString}";
        }

        public override string[] _GetConfigurationWarnings()
        {
            if (Query is null) return ["Query is missing!"];
            return [];
        }

        public override Status _Tick(double delta)
        {
            if (Query is null) return Status.Failure;
            if (Agent is Node3D agent)
            {
                var result = Query.Run<Variant>(agent);
                if (!ResultVar.IsEmpty())
                    Blackboard.SetVar(ResultVar, result);
                return Status.Success;
            }
            return Status.Failure;
        }
    }
}
