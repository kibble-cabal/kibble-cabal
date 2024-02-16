
using Godot;

public partial class MetaCS : Node
{
    public override void _Ready()
    {
        var item = new RItem { ID = "something", DisplayName = "Some Item" };
        ItemDB.Instance.Register(item);
        GD.PrintS(ItemDB.Instance.Find("something")?.DisplayName);
    }
}