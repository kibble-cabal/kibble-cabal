
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Godot.Collections;

namespace ContentLoader
{
    public class ZIP : IContentLoader
    {
        private readonly bool CaseSensitive;
        private readonly string Path;
        private readonly ZipReader Reader;
        public IEnumerable<string> IgnoredPaths { get; }

        public ZIP(string path, IEnumerable<string> ignoredPaths, bool caseSensitive = false)
        {
            Path = path;
            IgnoredPaths = ignoredPaths;
            CaseSensitive = caseSensitive;
            Reader = new();
            Reader.Open(Path);
        }

        public ZIP(string path, bool caseSensitive = false)
        {
            Path = path;
            IgnoredPaths = [];
            CaseSensitive = caseSensitive;
            Reader = new();
            Reader.Open(Path);
        }

        ~ZIP() => Reader.Close();

        public bool Exists(string path) => Reader.FileExists(path, CaseSensitive);

        public IEnumerable<string> GetFiles(string entryDir) => [.. Reader.GetFiles().Select(entryDir.PathJoin).Except(IgnoredPaths)];

        public AudioStream? LoadAudio(string path) => GD.BytesToVarWithObjects(LoadBytes(path)).TryAs<AudioStream>();

        public byte[]? LoadBytes(string path) => Exists(path) ? Reader.ReadFile(path, false) : null;

        public Font? LoadFont(string path) => GD.BytesToVarWithObjects(LoadBytes(path)).TryAs<Font>();

        public Texture2D? LoadImage(string path) => GD.BytesToVarWithObjects(LoadBytes(path)).TryAs<Texture2D>();

        public Resource? LoadResource(string path) => GD.BytesToVarWithObjects(LoadBytes(path)).TryAs<Resource>();

        public Script? LoadScript(string path) => GD.BytesToVarWithObjects(LoadBytes(path)).TryAs<Script>();

        public string? LoadString(string path) => LoadBytes(path)?.GetStringFromUtf8();
    }
}