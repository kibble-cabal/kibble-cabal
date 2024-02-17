using Godot;

[GlobalClass]
public sealed partial class RFate : ExtensibleResource
{
    private int _amount = 100;

    [Export]
    public int Amount
    {
        get => _amount;
        set => this.Set(ref _amount, value);
    }

    public void Earn(int delta) => Amount += delta;
    public void Lose(int delta) => Amount -= delta;
}