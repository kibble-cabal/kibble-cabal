using Godot;

[GlobalClass]
public partial class RItem : ExtensibleResource, IIdentifiable<StringName>
{
    private static class Keys
    {
        public const string Physics = "Physics";
        public const string Retail = "Retail";
    }

    private StringName _id = "";
    private string _displayName = "";
    private string _description = "";
    private Texture2D? _icon;

    [Export]
    public StringName ID
    {
        get => _id;
        set => this.Set(ref _id, value);
    }

    [Export]
    public StringName DisplayName
    {
        get => _displayName;
        set => this.Set(ref _displayName, value);
    }

    [Export]
    public string Description
    {
        get => _description;
        set => this.Set(ref _description, value);
    }

    [Export]
    public Texture2D? Icon
    {
        get => _icon;
        set => this.Set(ref _icon, value);
    }

    [Export]
    public RItemPhysics? Physics
    {
        get => GetSubresource<RItemPhysics>(Keys.Physics);
        set => SetSubresource(Keys.Physics, value);
    }

    [Export]
    public RItemRetail? Retail
    {
        get => GetSubresource<RItemRetail>(Keys.Retail);
        set => SetSubresource(Keys.Retail, value);
    }

    public RItemInstance Instantiate() => new RItemInstance { ItemID = ID };
}