using Godot;

[GlobalClass]
public partial class RInstructionContextAction : Resource, IPetContextAction
{

    private string _displayText = "";
    private BehaviorTree? _instructionTree;

    [Export]
    public string DisplayText
    {
        get => _displayText;
        set => this.Set(ref _displayText, value);
    }

    [Export]
    public BehaviorTree? InstructionTree
    {
        get => _instructionTree;
        set => this.Set(ref _instructionTree, value);
    }

    string IContextAction<IPetContextAction.Context>._GetDisplayText(IPetContextAction.Context ctx) => DisplayText;

    StringName[] IContextAction._GetMenuIdentifiers() => ["pet/interact"];

    void IContextAction<IPetContextAction.Context>._OnPress(IPetContextAction.Context ctx)
    {
        if (InstructionTree is BehaviorTree tree)
            ctx.Pet.Instructions.Add(tree);
    }

    static RInstructionContextAction()
    {
        #if TOOLS
        JSONSchema.GeneratorDB.Register(new JSONSchema.Generator
        {
            ClassName = nameof(RInstructionContextAction),
            Path = "res://docs/schemas/InstructionContextAction.schema.json",
            Title = "Instruction Context Action"
        });
        #endif
    }
}