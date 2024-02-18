using Godot;

public sealed partial class GameModeSubSystemBase : Node
{
    public static readonly PackedScene PauseUIScene = GD.Load<PackedScene>("res://expansion_packs/core/ui/scenes/paused_scene.tscn");
    public static readonly Node PauseUI = PauseUIScene.Instantiate();

    [Signal]
    public delegate void BeforeEnteredEventHandler(RGameMode mode);

    [Signal]
    public delegate void AfterEnteredEventHandler(RGameMode mode);

    [Signal]
    public delegate void BeforeExitedEventHandler(RGameMode mode);

    [Signal]
    public delegate void AfterExitedEventHandler(RGameMode mode);

    [Signal]
    public delegate void GameModeChangedEventHandler(RGameMode mode);


    public RGameMode? Current { get; private set; }
    public Node? State => Current?.StateInstance;

    public void To(RGameMode? gameMode)
    {
        Exit();
        if (gameMode is not null)
            Enter(gameMode);
        EmitSignal(SignalName.GameModeChanged);
    }

    private void Enter(RGameMode gameMode)
    {
        EmitSignal(SignalName.BeforeEntered, [gameMode]);
        gameMode.Enter();
        SetPaused();
        Current = gameMode;
        EmitSignal(SignalName.AfterEntered, [gameMode]);
    }

    private void Exit()
    {
        if (Current is RGameMode gameMode)
        {
            EmitSignal(SignalName.BeforeExited, [gameMode]);
            gameMode.Exit();
            SetPaused();
            Current = null;
            EmitSignal(SignalName.AfterExited, [gameMode]);
        }
    }

    private void SetPaused()
    {
        bool paused = Current?.WorldPaused ?? true;
        if (this.GetLocationRoot() is Node3D locationRoot)
            locationRoot.ProcessMode = paused ? ProcessModeEnum.Disabled : ProcessModeEnum.Inherit;
        UpdatePauseUI();
    }

    private void UpdatePauseUI()
    {
        if (Current is null) return;
        if (PauseUI.IsInsideTree() && !Current.WorldPaused)
            PauseUI.GetParent().RemoveChild(PauseUI);
        if (!PauseUI.IsInsideTree() && Current.WorldPaused)
            this.GetUIRoot()?.AddChild(PauseUI);
    }
}

public sealed partial class GameModeSubSystem : Singleton<GameModeSubSystemBase> { }