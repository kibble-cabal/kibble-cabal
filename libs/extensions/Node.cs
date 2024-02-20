using System.Linq;
using Godot;

public static class NodeExtensions
{
    public static class GroupName
    {
        public static readonly StringName UIRoot = "UIRoot";
        public static readonly StringName GameModeUIRoot = "GameModeUIRoot";
        public static readonly StringName LocationRoot = "LocationRoot";
    }

    public static void Set<[MustBeVariant] T>(this Node node, ref T prop, T value)
    {
        prop = value;
        node.NotifyPropertyListChanged();
    }

    public static bool CanQueueFree(this Node? node) => node != null
        && node.IsInsideTree()
        && !node.IsQueuedForDeletion();

    public static Node? GetUIRoot(this Node node) => node.IsInsideTree() ? node.GetTree().GetFirstNodeInGroup(GroupName.UIRoot) : null;
    public static UIStack? GetGameModeUIRoot(this Node node) => node.IsInsideTree() ? node.GetTree().GetFirstNodeInGroup(GroupName.GameModeUIRoot) as UIStack : null;
    public static Node3D? GetLocationRoot(this Node node) => node.IsInsideTree() ? node.GetTree().GetFirstNodeInGroup(GroupName.LocationRoot) as Node3D : null;
    public static void QueueFreeChildren(this Node node) => node.GetChildren().ToArray().Where(child => child.CanQueueFree()).ForEach(child => child.QueueFree());
}

public static class ThemeColor
{
    public static class ButtonFont
    {
        public static readonly StringName Base = "font_color";
        public static readonly StringName Disabled = "font_disabled_color";
        public static readonly StringName Hover = "font_hover_color";
        public static readonly StringName Pressed = "font_pressed_color";
        public static readonly StringName HoverPressed = "font_hover_pressed_color";
        public static readonly StringName Focus = "font_focus_color";
    }
}

public static class ButtonExtensions
{
    public static void OverrideColor(this Button button, Color color) => button.AddThemeColorOverride(ThemeColor.ButtonFont.Base, color);
    public static void OverrideDisabledColor(this Button button, Color color) => button.AddThemeColorOverride(ThemeColor.ButtonFont.Disabled, color);
    public static void OverrideHoverColor(this Button button, Color color) => button.AddThemeColorOverride(ThemeColor.ButtonFont.Hover, color);
    public static void OverridePressedColor(this Button button, Color color) => button.AddThemeColorOverride(ThemeColor.ButtonFont.Pressed, color);
    public static void OverrideHoverPressedColor(this Button button, Color color) => button.AddThemeColorOverride(ThemeColor.ButtonFont.HoverPressed, color);
    public static void OverrideFocusColor(this Button button, Color color) => button.AddThemeColorOverride(ThemeColor.ButtonFont.Focus, color);
}