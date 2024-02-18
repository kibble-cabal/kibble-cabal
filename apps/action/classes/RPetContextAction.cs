using Godot;


[GlobalClass]
public partial class RPetContextAction : RContextAction<RPetContextAction.Context>
{
    public class Context
    {
        public required CharacterBody3D Node;
        public required RPet Pet;
    }

    private StringName _id = "";

    [Export]
    public override StringName ID
    {
        get => _id;
        set => this.Set(ref _id, value);
    }

    protected override string _GetDisplayText(Context? ctx) => "";

    protected override StringName[] _GetMenuIdentifiers() => ["pet/interact"];
}