using System.Linq;
using Godot;

[GlobalClass]
public partial class RGameMode : ExtensibleResource, IIdentifiable<StringName>
{
    public const string UISceneGroupName = "GameModeUIScene";

    private StringName _id = "";
    private bool _worldPaused = true;
    private Script? _stateScript;
    private Color _uiColor;
    private PackedScene? _uiScene;
    private int? _uiMenuIndex;
    private Texture2D? _uiIcon;

    public StringName ID => _id;
    public Node? StateInstance { get; private set; }

    [Export]
    public string Name
    {
        get => _id;
        set => this.Set(ref _id, value);
    }

    [Export]
    public bool WorldPaused
    {
        get => _worldPaused;
        set => this.Set(ref _worldPaused, value);
    }

    [Export]
    public Script? StateScript
    {
        get => _stateScript;
        set => this.Set(ref _stateScript, value);
    }

    [ExportGroup("UI", "UI")]

    [Export]
    public Color UIColor
    {
        get => _uiColor;
        set => this.Set(ref _uiColor, value);
    }

    [Export]
    public PackedScene? UIScene
    {
        get => _uiScene;
        set => this.Set(ref _uiScene, value);
    }

    [Export]
    public int UIMenuIndex
    {
        get => _uiMenuIndex ?? -1;
        set => this.Set(ref _uiMenuIndex, value);
    }

    [Export]
    public Texture2D? UIIcon
    {
        get => _uiIcon;
        set => this.Set(ref _uiIcon, value);
    }

    public void Enter()
    {
        if (GameModeSubSystem.Instance is GameModeSubSystemBase system)
        {
            if (StateScript is Script script)
            {
                StateInstance = script.New() as Node;
                if (StateInstance != null)
                    system.AddChild(StateInstance);
            }
            if (system.GetGameModeUIRoot() is UIStack node && UIScene != null)
            {
                var instance = UIScene.Instantiate<Control>();
                instance.AddToGroup(UISceneGroupName);
                node.Push(instance);
            }
        }
    }

    public void Exit()
    {
        if (GameModeSubSystem.Instance is GameModeSubSystemBase system)
        {
            if (StateInstance is not null && StateInstance.CanQueueFree())
                StateInstance.QueueFree();
            if (system.GetGameModeUIRoot() is UIStack node)
                node.Clear();
        }
    }


}