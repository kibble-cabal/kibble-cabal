using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace ContentLoader
{
    public readonly struct File : IContentLoader
    {
        public IEnumerable<string> IgnoredPaths { get; }

        public File(IEnumerable<string> ignoredPaths) => IgnoredPaths = ignoredPaths;
        public File() => IgnoredPaths = [];

        public readonly bool Exists(string path)
        {
            if (path.GetExtension().Length == 0)
                return DirAccess.DirExistsAbsolute(path);
            else return FileAccess.FileExists(path);
        }

        public readonly IEnumerable<string> GetFiles(string entryDir) => [
            ..DirAccess.GetDirectoriesAt(entryDir).Select(entryDir.PathJoin).Except(IgnoredPaths).SelectMany(GetFiles),
            ..DirAccess.GetFilesAt(entryDir).Select(entryDir.PathJoin).Except(IgnoredPaths)
        ];

        public readonly byte[]? LoadBytes(string path) => FileAccess.GetFileAsBytes(path);
        public readonly string? LoadString(string path) => FileAccess.GetFileAsString(path);
        public readonly Resource? LoadResource(string path) => ResourceLoader.Load(path);
        public readonly Script? LoadScript(string path) => IContentLoader.TryLoad<Script>(path);
        public readonly Texture2D? LoadImage(string path) => IContentLoader.TryLoad<Texture2D>(path);
        public readonly AudioStream? LoadAudio(string path) => IContentLoader.TryLoad<AudioStream>(path);
        public readonly Font? LoadFont(string path) => IContentLoader.TryLoad<Font>(path);
    }
}