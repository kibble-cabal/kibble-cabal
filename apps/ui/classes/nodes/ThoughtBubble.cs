using System.Threading.Tasks;
using Godot;

[Tool]
[GlobalClass]
public partial class ThoughtBubble : Node2D, IControl3D
{
    public static readonly StringName UIGroupName = "UI";
    public static readonly StringName GroupName = "ThoughtBubble";

    public static readonly Shader BubbleShader = GD.Load<Shader>("res://content/shaders/canvas_item/thought_bubble_2.gdshader");
    public static readonly Material WavyTextMaterial = GD.Load<Material>("res://content/materials/wavy_text.tres");

    private string _text = "";
    private Font? _font;
    private int _fontSize;
    private float _maxWidth;
    private Color _color = Colors.White;
    private Color _backgroundColor = Colors.White * 0.1f;
    private bool _center = true;

    [Export(PropertyHint.MultilineText)]
    public string Text
    {
        get => _text;
        set
        {
            _text = value;
            QueueRedrawAll();
        }
    }

    [Export]
    public Font? Font
    {
        get => _font;
        set
        {
            _font = value;
            QueueRedrawAll();
        }
    }

    [Export]
    public int FontSize
    {
        get => _fontSize;
        set
        {
            _fontSize = value;
            QueueRedrawAll();
        }
    }

    [Export]
    public float MaxWidth
    {
        get => _maxWidth;
        set
        {
            _maxWidth = value;
            QueueRedrawAll();
        }
    }

    [Export]
    public Color Color
    {
        get => _color;
        set
        {
            _color = value;
            QueueRedrawAll();
        }
    }

    [Export]
    public Color BackgroundColor
    {
        get => _backgroundColor;
        set
        {
            _backgroundColor = value;
            QueueRedrawAll();
        }
    }

    [Export]
    public bool Center
    {
        get => _center;
        set
        {
            _center = value;
            QueueRedrawAll();
        }
    }

    public Vector2 Size => GetBackgroundSize();

    public Vector3 LocalPosition { get; set; }
    public Vector2 ScreenOffset { get; set; }

    private Node? Parent;
    private Camera3D? Camera;

    public ThoughtBubble()
    {
        AddToGroup(UIGroupName);
        AddToGroup(GroupName);
    }

    public ThoughtBubble(string text, float duration = 3, float maxWidth = -1) : this()
    {
        Text = text;
        MaxWidth = maxWidth;
        _ = Appear(duration);
    }

    private ShaderMaterial Mat = new();
    private ColorRect Background = new();
    private Label Foreground = new();

    public override void _EnterTree()
    {
        Parent = GetParent();
        Camera = GetViewport()?.GetCamera3D();
        Mat.Shader = BubbleShader;
        Mat.SetShaderParameter("seed", GD.RandRange(0, 1000));
        Mat.SetShaderParameter("num_bubbles", GD.RandRange(7.5, 9.5));
        AddChild(Background);
        AddChild(Foreground);
    }

    public override void _Notification(int what)
    {
        if (what.IsNotification(NotificationReady, NotificationEnterCanvas, NotificationDraw, NotificationVisibilityChanged))
            QueueRedrawAll();
    }

    public override void _Process(double delta) => IControl3D.ProcessPosition(this, Parent, Camera);

    private void Update()
    {
        //  Update background
        Background.Color = Colors.Black;
        Background.Material = Mat;
        Background.CustomMinimumSize = GetBackgroundSize();
        Background.Position = Center ? -Background.CustomMinimumSize / 2 : Vector2.Zero;
        Background.ResetSize();
        Mat.SetShaderParameter("color", BackgroundColor);

        // Update foreground
        Foreground.Text = Text;
        Foreground.UseParentMaterial = true;
        Foreground.OverrideColor(Color);
        Foreground.OverrideFont(GetFont());
        Foreground.OverrideFontSize(GetFontSize());
        Foreground.AutowrapMode = TextServer.AutowrapMode.WordSmart;
        Foreground.CustomMinimumSize = GetForegroundSize();
        Foreground.ResetSize();
        Foreground.Position = Background.Position + ((Background.Size - Foreground.Size) / 2);

        Material = WavyTextMaterial;
    }

    private Font GetFont() => Font ?? ThemeDB.GetProjectTheme().DefaultFont;
    private int GetFontSize() => FontSize > 0 ? FontSize : ThemeDB.GetProjectTheme().DefaultFontSize;
    private float GetWidth() => MaxWidth > 0 ? MaxWidth : 600;

    private Vector2 GetForegroundSize() => GetFont().GetMultilineStringSize(Text, HorizontalAlignment.Center, GetWidth() * 0.707f, GetFontSize());

    private Vector2 GetBackgroundSize()
    {
        var width = GetWidth();
        var minHeight = Mathf.Max(width, GetForegroundSize().Y);
        return Mathf.Max(width, minHeight).ToVector2() * 1.3f;
    }

    private void ResetSize()
    {
        Background.CustomMinimumSize = GetBackgroundSize();
        Foreground.CustomMinimumSize = GetForegroundSize();
        Background.ResetSize();
        Foreground.ResetSize();
    }

    private void QueueRedrawAll()
    {
        Update();
        Background.QueueRedraw();
        Foreground.QueueRedraw();
    }

    public async Task Appear(float duration = 3)
    {
        ResetSize();
        Scale *= 0;
        if (!IsNodeReady())
            await ToSignal(this, Node.SignalName.Ready);

        var tween = CreateTween()
            .SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Back);
        tween.TweenProperty(this, new NodePath(Control.PropertyName.Scale), Vector2.One, 0.25);
        await ToSignal(tween, Tween.SignalName.Finished);

        if (duration > 0 && this.CanQueueFree())
        {
            await ToSignal(GetTree().CreateTimer(duration), Timer.SignalName.Timeout);
            await Destroy();
        }
    }

    public async Task Destroy()
    {
        if (!this.CanQueueFree()) return;
        var tween = CreateTween()
            .SetEase(Tween.EaseType.In)
            .SetTrans(Tween.TransitionType.Back);
        tween.TweenProperty(this, new NodePath(Control.PropertyName.Scale), Vector2.Zero, 0.25);
        await ToSignal(tween, Tween.SignalName.Finished);
    }
}