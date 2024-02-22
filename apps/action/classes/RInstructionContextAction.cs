using Godot;

[GlobalClass]
public partial class RInstructionContextAction : Resource, IPetContextAction
{

    private string _displayText = "";
    private Resource? _instructionTree;

    [Export]
    public string DisplayText
    {
        get => _displayText;
        set => this.Set(ref _displayText, value);
    }

    [Export]
    public Resource? InstructionTree
    {
        get => _instructionTree;
        set => this.Set(ref _instructionTree, value);
    }

    string IContextAction<IPetContextAction.Context>._GetDisplayText(IPetContextAction.Context ctx) => DisplayText;

    StringName[] IContextAction._GetMenuIdentifiers() => ["pet/interact"];

    void IContextAction<IPetContextAction.Context>._OnPress(IPetContextAction.Context ctx)
    {
        if (InstructionTree is Resource tree)
            ctx.Pet.Instructions.Add(tree);
    }
}