using System.Linq;
using Godot;

public class ExpansionPackLoaderBase
{
    private static readonly string[] DirsToSearch = [
        "res://"
    ];

    private static readonly string[] PCKDirsToSearch = [
        "res://",
        "user://"
    ];

    private static readonly string[] DirsToSkip = [
        "res://addons",
        "res://apps",
        "res://content"
    ];

    public RExpansionPack[] LoadPacks(bool verbose = true)
    {
        IContentLoader loader = new ContentLoader.File(DirsToSkip);

        var pckFiles = PCKDirsToSearch.SelectMany(dir => loader.GetFilesByExtension(dir, "expansion.pck", "expansion.zip"));
        if (verbose) this.Print($"Discovered PCK Files: {pckFiles.ToGodotArray()}");

        if (OperatingSystem.IsStandalone) pckFiles.ForEach(file =>
        {
            if (!ProjectSettings.LoadResourcePack(file))
                GD.PushError($"Error opening expansion pack: {file}");
        });
        else if (verbose && pckFiles.Any()) this.Print("Can't unpackage PCK files in editor build, skipping...");

        var resourceFiles = DirsToSearch.SelectMany(dir => loader.GetFilesByExtension(dir, "expansion.tres", "expansion.res"));
        if (verbose) this.Print($"Discovered resource files: {resourceFiles.ToGodotArray()}");

        return [.. resourceFiles.Select(IContentLoader.TryLoad<RExpansionPack>).WhereNotNull()];
    }

    public override string ToString() => "ExpansionPackLoader";
}

public class ExpansionPackLoader : Singleton<ExpansionPackLoaderBase>
{
    public static RExpansionPack[] LoadPacks(bool verbose = true) => Instance.LoadPacks(verbose);
}