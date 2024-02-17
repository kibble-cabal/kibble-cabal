using Godot;

[GlobalClass]
public sealed partial class RPet : ExtensibleResource
{
    private string _name = "";
    private int _birthDate;

    // TODO: Instructions

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
}