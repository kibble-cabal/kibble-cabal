using System.Collections.Generic;
using System.Linq;

public class ModLoaderBase
{
    private static readonly string[] DirsToSearch = ["user://mods"];

    public RMod[] LoadMods(bool verbose = true)
    {
        IContentLoader loader = new ContentLoader.File();

        var zipFiles = DirsToSearch.SelectMany(dir => loader.GetFilesByExtension(dir, "mod.zip"));
        if (verbose) this.Print($"Discovered mod ZIP Files: {zipFiles.ToGodotArray()}");

        // TODO: JSON parsing
        List<RMod> resources = [];
        foreach (var zipPath in zipFiles ?? [])
        {
            IContentLoader zipLoader = new ContentLoader.ZIP(zipPath);
            var resourceFiles = DirsToSearch.SelectMany(dir => loader.GetFilesByExtension(dir, "mod.tres", "mod.res"));
            if (verbose) this.Print($"Discovered mod files in {zipPath}: {resourceFiles.ToGodotArray()}");
            resources.AddRange(resourceFiles.Select(IContentLoader.TryLoad<RMod>).WhereNotNull());
        }

        return [.. resources];
    }

    public override string ToString() => "ModLoader";
}

public class ModLoader : Singleton<ModLoaderBase>
{
    public static RMod[] LoadMods(bool verbose = true) => Instance.LoadMods(verbose);
}