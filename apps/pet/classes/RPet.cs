using Godot;
using Godot.Collections;

[GlobalClass]
public sealed partial class RPet : ExtensibleResource
{
    private string _name = "";
    private int _birthDate;
    private Array<Resource> _instructions = [];

    public string Name
    {
        get => _name;
        set => this.Set(ref _name, value);
    }

    public int BirthDate
    {
        get => _birthDate;
        set => this.Set(ref _birthDate, value);
    }

    public Array<Resource> Instructions
    {
        get => _instructions;
        set => this.Set(ref _instructions, value);
    }
}