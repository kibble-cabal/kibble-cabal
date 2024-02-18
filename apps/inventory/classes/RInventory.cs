using Godot;
using Godot.Collections;

[GlobalClass]
public partial class RInventory : ExtensibleResource
{
    private Array<RItemInstance> _itemInstances = [];
    private int _maxCapacity = -1;

    [Export]
    public int MaxCapacity
    {
        get => _maxCapacity;
        set => this.Set(ref _maxCapacity, value);
    }

    [Export]
    public Array<RItemInstance> ItemInstances
    {
        get => _itemInstances;
        set => this.Set(ref _itemInstances, value);
    }

    public bool IsFull() => MaxCapacity > 0 && ItemInstances.Count >= MaxCapacity;
}