using System;
using Godot;


public abstract partial class RContextAction : Resource, IIdentifiable<StringName>
{
    public abstract StringName ID { get; set; }

    protected abstract string _GetDisplayText<Ctx>(Ctx? ctx) where Ctx : class;
    protected abstract StringName[] _GetMenuIdentifiers();
    protected virtual bool _IsVisible<Ctx>(Ctx? ctx) where Ctx : class => true;
    protected virtual void _OnPress<Ctx>(Ctx? ctx) where Ctx : class { }
    protected virtual void _Update<Ctx>(Button button, Ctx? ctx) where Ctx : class => button.Text = GetDisplayText(ctx);

    public string GetDisplayText<Ctx>(Ctx? ctx) where Ctx : class => _GetDisplayText(ctx);
    public StringName[] GetMenuIdentifiers() => _GetMenuIdentifiers();
    public bool IsVisible<Ctx>(Ctx? ctx) where Ctx : class => _IsVisible(ctx);

    protected void OnPress<Ctx>(Ctx? ctx) where Ctx : class => _OnPress(ctx);

    public void Update<Ctx>(Button button, Ctx? ctx) where Ctx : class
    {
        button.Visible = IsVisible(ctx);
        button.DisconnectAllFromTarget(BaseButton.SignalName.Pressed, this);
        button.Connect(BaseButton.SignalName.Pressed, Callable.From(() => OnPress(ctx)));
        _Update(button, ctx);
    }

    public Button Render<Ctx>(Ctx? ctx) where Ctx : class
    {
        var button = new Button();
        button.Connect(BaseButton.SignalName.Pressed, Callable.From(() => OnPress(ctx)));
        return button;
    }
}

public abstract partial class RContextAction<Ctx> : RContextAction where Ctx : class
{
    protected sealed override string _GetDisplayText<C>(C? ctx) where C : class => _GetDisplayText(ctx as Ctx);
    protected sealed override bool _IsVisible<C>(C? ctx) where C : class => _IsVisible(ctx as Ctx);
    protected sealed override void _OnPress<C>(C? ctx) where C : class => _OnPress(ctx as Ctx);
    protected sealed override void _Update<C>(Button button, C? ctx) where C : class => _Update(button, ctx as Ctx);

    protected abstract string _GetDisplayText(Ctx? ctx);
    protected virtual bool _IsVisible(Ctx? ctx) => true;
    protected virtual void _OnPress(Ctx? ctx) { }
    protected virtual void _Update(Button button, Ctx? ctx) => button.Text = GetDisplayText(ctx);

    public string GetDisplayText(Ctx? ctx) => _GetDisplayText(ctx);
    public bool IsVisible(Ctx? ctx) => _IsVisible(ctx);

    protected void OnPress(Ctx? ctx) => _OnPress(ctx);

    public void Update(Button button, Ctx? ctx)
    {
        button.Visible = IsVisible(ctx);
        button.DisconnectAllFromTarget(BaseButton.SignalName.Pressed, this);
        button.Connect(BaseButton.SignalName.Pressed, Callable.From(() => OnPress(ctx)));
        _Update(button, ctx);
    }

    public Button Render(Ctx? ctx)
    {
        var button = new Button();
        button.Connect(BaseButton.SignalName.Pressed, Callable.From(() => OnPress(ctx)));
        return button;
    }
}