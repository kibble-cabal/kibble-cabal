using Godot;


[GlobalClass]
public partial class RPetContextAction : RContextAction<RPetContextAction.Context>
{
    public class Context
    {
        public required CharacterBody3D Node;
        public required RPet Pet;
    }

    protected override string _GetDisplayText(Context? ctx) => "";

    protected override StringName[] _GetMenuIdentifiers() => ["pet/interact"];
}