using Godot;
using Godot.Collections;

[GlobalClass]
public sealed partial class RPet : ExtensibleResource
{
    private string _name = "";
    private int _birthDate;
    private StringName _animalID = "";
    private Array<Resource> _instructions = [];

    [Export]
    public string Name
    {
        get => _name;
        set => this.Set(ref _name, value);
    }

    [Export]
    public StringName AnimalID
    {
        get => _animalID;
        set => this.Set(ref _animalID, value);
    }

    [Export]
    public int BirthDate
    {
        get => _birthDate;
        set => this.Set(ref _birthDate, value);
    }

    [Export]
    public Array<Resource> Instructions
    {
        get => _instructions;
        set => this.Set(ref _instructions, value);
    }

    [Export]
    public Vector3 Position;

    public RAnimal? GetAnimal() => AnimalDB.Instance.Find(AnimalID);
}