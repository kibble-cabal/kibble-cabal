using System;
using System.Threading.Tasks;
using Godot;

namespace KibbleCabal.Apps.GameMode
{
    public partial class GameModeButton : Button
    {
        private static readonly ShaderMaterial BaseFocusMaterial = GD.Load<ShaderMaterial>("res://apps/game_mode/resources/game_mode_button_focus_material.tres");

        private static class Params
        {
            public static class Name
            {
                public static readonly StringName UseColor1 = "use_color_1";
                public static readonly StringName UseColor2 = "use_color_2";
                public static readonly StringName ReplaceColor1 = "replace_color_1";
                public static readonly StringName ReplaceColor2 = "replace_color_2";
            }
            public static class Path
            {
                public static readonly NodePath UseColor1 = $"shader_parameter/{Name.UseColor1}";
                public static readonly NodePath UseColor2 = $"shader_parameter/{Name.UseColor2}";
                public static readonly NodePath ReplaceColor1 = $"shader_parameter/{Name.ReplaceColor1}";
                public static readonly NodePath ReplaceColor2 = $"shader_parameter/{Name.ReplaceColor2}";
            }
            public static class Default
            {
                public static readonly Color Color = new("#0d0d0d");
            }
        }

        private RGameMode? _gameMode;
        private TextureRect? IconTexture;
        private ColorRect? ShadowRect;

        [Export]
        public RGameMode? GameMode
        {
            get => _gameMode;
            set
            {
                _gameMode?.TryDisconnectChanged(Callable.From(Update));
                value?.TryConnectChanged(Callable.From(Update));
                _gameMode = value;
                Update();
            }
        }

        private ShaderMaterial FocusMaterial = (ShaderMaterial)BaseFocusMaterial.Duplicate();

        public override void _Ready()
        {
            SizeFlagsHorizontal = SizeFlags.ShrinkBegin;
            SizeFlagsVertical = SizeFlags.ShrinkCenter;
            IconTexture = GetNode<TextureRect>("TextureRect");
            ShadowRect = GetNode<ColorRect>("ShadowRect");
            Update();
        }

        private void Update()
        {
            if (GameMode is null) return;
            Text = GameMode.Name.ToLower();
            UpdateColor();
            UpdateIconTexture();
            UpdateShadow();
        }

        private void UpdateColor()
        {
            if (GameMode is null) return;
            Action<Color>[] methods = [this.OverrideColor, this.OverrideHoverColor, this.OverridePressedColor, this.OverrideHoverPressedColor, this.OverrideFocusColor];
            methods.ForEach(method => method(GameMode.UIColor));
            this.OverrideDisabledColor(new Color(GameMode.UIColor, 0.5f));
        }

        private void UpdateIconTexture()
        {
            if (GameMode is null || IconTexture is null) return;
            IconTexture.Texture = GameMode.UIIcon;
            IconTexture.Modulate = GameMode.UIColor;
        }

        private void UpdateShadow()
        {
            if (GameMode is null || ShadowRect is null) return;
            ShadowRect.CustomMinimumSize = Size;
            ShadowRect.ResetSize();
            ShadowRect.Position = Position + new Vector2(-30, 10);
        }

        private async Task TweenFocus()
        {
            if (GameMode is null) return;
            var tween = CreateTween().SetParallel();
            tween.TweenProperty(Material, Params.Path.UseColor1, Params.Default.Color, 0.075);
            tween.TweenProperty(Material, Params.Path.UseColor2, GameMode.UIColor, 0.075);
            tween.TweenProperty(this, "scale", new Vector2(1.1f, 1.1f), 0.15);
            await ToSignal(tween, Tween.SignalName.Finished);
        }

        private async Task TweenUnfocus()
        {
            if (GameMode is null) return;
            var tween = CreateTween().SetParallel().SetEase(Tween.EaseType.InOut).SetTrans(Tween.TransitionType.Sine);
            tween.TweenProperty(Material, Params.Path.UseColor2, Params.Default.Color, 0.075);
            tween.TweenProperty(Material, Params.Path.UseColor1, GameMode.UIColor, 0.075);
            tween.TweenProperty(this, "scale", new Vector2(1f, 1f), 0.15);
            await ToSignal(tween, Tween.SignalName.Finished);
        }

        public async Task OnFocusEntered()
        {
            if (GameMode is null) return;
            FocusMaterial.SetShaderParameter(Params.Name.ReplaceColor1, GameMode.UIColor);
            FocusMaterial.SetShaderParameter(Params.Name.ReplaceColor2, Params.Default.Color);
            FocusMaterial.SetShaderParameter(Params.Name.UseColor1, GameMode.UIColor);
            FocusMaterial.SetShaderParameter(Params.Name.UseColor2, Params.Default.Color);
            await TweenFocus();
        }

        public async Task OnFocusExited()
        {
            if (GameMode is null) return;
            Material = null;
            await TweenUnfocus();
        }

        public void OnPressed() => GameModeSubSystem.To(GameMode);
    }
}