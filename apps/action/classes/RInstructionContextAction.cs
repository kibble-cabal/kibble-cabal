using Godot;

[GlobalClass]
public partial class RInstructionContextAction : RPetContextAction
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

    protected override string _GetDisplayText(Context? ctx) => DisplayText;

    protected override StringName[] _GetMenuIdentifiers() => ["pet/interact"];

    protected override void _OnPress(Context? ctx)
    {
        if (ctx is Context context && InstructionTree is Resource tree)
            context.Pet.Instructions.Add(tree);
    }
}