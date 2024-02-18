using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

public interface IContentLoader
{
    IEnumerable<string> IgnoredPaths { get; }

    bool Exists(string path);
    IEnumerable<string> GetFiles(string entryDir);
    IEnumerable<string> GetFilesByExtension(string entryDir, params string[] extensions) => GetFiles(entryDir).Where(path => path.HasExtension(extensions));

    byte[]? LoadBytes(string path);
    string? LoadString(string path);
    Resource? LoadResource(string path);
    Script? LoadScript(string path);
    Texture2D? LoadImage(string path);
    AudioStream? LoadAudio(string path);
    Font? LoadFont(string path);

    Dictionary? LoadJSON(string path)
    {
        if (LoadString(path) is string str) return Json.ParseString(str).TryAs<Dictionary>();
        return null;
    }

    static R? TryLoad<R>(string path) where R : Resource
    {
        try
        {
            return ResourceLoader.Load<R>(path);
        }
        catch
        {
            return null;
        }
    }
}