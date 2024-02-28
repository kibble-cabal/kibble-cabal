using Godot;

[Tool]
[GlobalClass]
public partial class RItemRetail : ExtensibleResource
{
    [Export] public int BuyPrice;

    [Export] public int BaseSellPrice;

    [Export] public float DepreciationRate = 0.5f;
    
    static RItemRetail()
    {
        #if TOOLS
        JSON.Schema.GeneratorDB.Register(new JSON.Schema.Generator
        {
            ClassName = nameof(RItemRetail),
            Path = "res://docs/schemas/ItemRetail.schema.json",
            Title = "Item Retail Data"
        });
        #endif
    }
}