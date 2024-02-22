using Godot;

public interface IContextAction
{
    protected StringName[] _GetMenuIdentifiers();
    StringName[] GetMenuIdentifiers() => _GetMenuIdentifiers();
}

public interface IContextAction<Ctx> : IContextAction
{
    protected string _GetDisplayText(Ctx ctx);
    protected bool _IsVisible(Ctx ctx) => true;
    protected void _OnPress(Ctx ctx);
    protected void _Update(Button button, Ctx ctx) { }

    string GetDisplayText(Ctx ctx) => _GetDisplayText(ctx);
    bool IsVisible(Ctx ctx) => _IsVisible(ctx);
    protected void OnPress(Ctx ctx) => _OnPress(ctx);
    void Update(Button button, Ctx ctx)
    {
        button.Text = GetDisplayText(ctx);
        button.Visible = IsVisible(ctx);
        _Update(button, ctx);
    }
    Button Render(Ctx ctx)
    {
        var button = new Button();
        button.Pressed += () => OnPress(ctx);
        button.Connect(Node.SignalName.Ready, Callable.From(() => Update(button, ctx)), (uint)GodotObject.ConnectFlags.OneShot);
        return button;
    }
}

public interface IPetContextAction : IContextAction<IPetContextAction.Context>
{
    public class Context
    {
        public required CharacterBody3D Node;
        public required RPet Pet;
    }
}
