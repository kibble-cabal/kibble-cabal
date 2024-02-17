
using Godot;

public partial class MetaCS : Node
{
    public override void _EnterTree()
    {
        AddChild(SaveSubSystem.Instance);
        AddChild(LocationSubSystem.Instance);
    }

    public override void _Ready()
    {
        LocationDB.Register(GD.Load<RLocation>("res://expansion_packs/core/location/resources/Island.tres"));
        LocationSubSystem.To("Island");
    }
}