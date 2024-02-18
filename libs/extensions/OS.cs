public static class OperatingSystem
{
    public static class FeatureName
    {
        public const string Standalone = "standalone";
    }

    public static bool IsStandalone => Godot.OS.HasFeature(FeatureName.Standalone);
}