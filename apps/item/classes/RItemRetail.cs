using Godot;

public partial class RItemRetail : ExtensibleResource
{
    private int _buyPrice;
    private int _baseSellPrice;
    private float _depreciationRate = 0.5f;

    [Export]
    public int BuyPrice
    {
        get => _buyPrice;
        set => this.Set(ref _buyPrice, value);
    }

    [Export]
    public int BaseSellPrice
    {
        get => _baseSellPrice;
        set => this.Set(ref _baseSellPrice, value);
    }

    [Export]
    public float DepreciationRate
    {
        get => _depreciationRate;
        set => this.Set(ref _depreciationRate, value);
    }
}