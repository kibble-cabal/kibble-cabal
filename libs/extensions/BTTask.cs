using Godot;
using BB;

namespace BTContext
{
    public static class VarName
    {
        public static readonly StringName IsInstruction = "context/is_instruction";
    }
}

public interface IVerbose
{
    bool Verbose { get; set; }
}

public static class BTTaskExtensions
{
    public static BT.Status FailWithWarning(this BTTask task, params Variant[] warnings)
    {
        task.Warn(warnings);
        return BT.Status.Failure;
    }

    public static BT.Status FailWithWarning<T>(this T task, params Variant[] warnings) where T : BTTask, IVerbose
    {
        task.Warn(warnings);
        return BT.Status.Failure;
    }

    public static void Warn(this BTTask task, params Variant[] warnings)
    {
        GD.PushWarning(task._GenerateName().Brackets(), " ", warnings);
        GD.PrintRich(task._GenerateName().Brackets().Yellow(), " ", warnings.JoinToString().Yellow());
    }

    public static void Warn<T>(this T task, params Variant[] warnings) where T : BTTask, IVerbose
    {
        if (task.Verbose) (task as BTTask).Warn(warnings);
    }

    public static BT.Status AsStatus(this bool condition) => condition ? BT.Status.Success : BT.Status.Failure;
}

public static class BlackboardExtensions
{
    public static bool IsInstruction(this Blackboard blackboard) => (bool)blackboard.GetVar(BTContext.VarName.IsInstruction, false);
}