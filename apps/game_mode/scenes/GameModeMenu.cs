using Godot;

namespace KibbleCabal.Apps.GameMode;

public partial class GameModeMenu : Control
{
	private static class Nodes
	{
		public static readonly NodePath Background = "Background";
		public static readonly NodePath Container = "GameModeButtonContainer";
		public static readonly NodePath OpenButton = "ButtonContainer/OpenButton";
		public static readonly NodePath CloseButton = "ButtonContainer/CloseButton";
		public static readonly NodePath TapArea = "TapArea";
	}

	private static class Properties
	{
		public static readonly NodePath Scale = "scale";
		public static readonly NodePath RotationDegrees = "rotation_degrees";
		public static readonly NodePath DegreeMin = "DegreeMin";
		public static readonly NodePath DegreeMax = "DegreeMax";
	}
	
	private GameModeButtonContainer? Container;
	private ColorRect? Background;
	private Button? OpenButton;
	private Button? CloseButton;
	private TouchScreenButton? TapArea;
	
	public override void _Ready()
	{
		Container = GetNode<GameModeButtonContainer>(Nodes.Container);
		Background = GetNode<ColorRect>(Nodes.Background);
		OpenButton = GetNode<Button>(Nodes.OpenButton);
		CloseButton = GetNode<Button>(Nodes.CloseButton);
		TapArea = GetNode<TouchScreenButton>(Nodes.TapArea);

		if (OpenButton is not null)
		{
			OpenButton.Pressed += Open;
			OpenButton.PivotOffset = OpenButton.Size / 2;
		}
		
		if (CloseButton is not null)
		{
			CloseButton.Pressed += Close;
			CloseButton.PivotOffset = CloseButton.Size / 2;
		}

		if (TapArea is not null)
		{
			TapArea.Pressed += Close;
		}

		Close();
	}

	private void Open()
	{
		if (Container is null || Background is null || OpenButton is null || CloseButton is null) return;

		var tween = GetTree().CreateTween().SetParallel().SetTrans(Tween.TransitionType.Back);
		
		// Tween open/close buttons
		CloseButton.RotationDegrees = -180.0f;
		tween.TweenProperty(OpenButton, Properties.RotationDegrees, 180.0f, 0.25f);
		tween.TweenProperty(OpenButton, Properties.Scale, Vector2.Zero, 0.25f);
		tween.TweenProperty(CloseButton, Properties.RotationDegrees, 0.0f, 0.25f).SetDelay(0.1f);
		tween.TweenProperty(CloseButton, Properties.Scale, Vector2.One, 0.25f).SetDelay(0.1f);
		
		// Tween background
		Background.RotationDegrees = -210.0f;
		tween.SetEase(Tween.EaseType.Out);
		tween.TweenProperty(Background, Properties.RotationDegrees, -30.0f, 0.3f);
		tween.TweenProperty(Background, Properties.Scale, Vector2.One, 0.3f);
		
		// Tween container
		GetTree().CreateTimer(0.1f).Timeout += () => Container.Visible = true;
		Container.DegreeMin = -20.0f;
		Container.DegreeMax = 0.0f;
		tween.SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(Container, Properties.DegreeMin, 0.0f, 0.2f).SetDelay(0.1f);
		tween.TweenProperty(Container, Properties.DegreeMax, 110.0f, 0.2f).SetDelay(0.1f);

		if (TapArea is not null)
			TapArea.Visible = true;
	}
	
	private void Close()
	{
		if (Container is null || Background is null || OpenButton is null || CloseButton is null) return;

		var tween = GetTree().CreateTween().SetParallel().SetTrans(Tween.TransitionType.Back);
		
		// Tween open/close buttons
		OpenButton.RotationDegrees = -180.0f;
		tween.TweenProperty(CloseButton, Properties.RotationDegrees, 180.0f, 0.25f);
		tween.TweenProperty(CloseButton, Properties.Scale, Vector2.Zero, 0.25f);
		tween.TweenProperty(OpenButton, Properties.RotationDegrees, 0.0f, 0.25f).SetDelay(0.1f);
		tween.TweenProperty(OpenButton, Properties.Scale, Vector2.One, 0.25f).SetDelay(0.1f);
		
		// Tween background
		tween.SetEase(Tween.EaseType.In);
		tween.TweenProperty(Background, Properties.RotationDegrees, 150.0f, 0.3f);
		tween.TweenProperty(Background, Properties.Scale, Vector2.Zero, 0.3f);
		
		// Tween container
		tween.SetTrans(Tween.TransitionType.Sine);
		tween.TweenProperty(Container, Properties.DegreeMin, 40.0f, 0.1f).SetDelay(0.1f);
		tween.TweenProperty(Container, Properties.DegreeMax, 120.0f, 0.1f).SetDelay(0.1f);
		GetTree().CreateTimer(0.2f).Timeout += () => Container.Visible = false;
		
		if (TapArea is not null)
			TapArea.Visible = false;
	}
}