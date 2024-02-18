using System.IO;
using System.Linq;
using Godot;

public static class StringExtensions
{
    public static string GetFullExtension(this string path)
    {
        var ret = "";
        while (true)
        {
            var ext = Path.GetExtension(path);
            if (string.IsNullOrEmpty(ext)) break;
            path = path[..^ext.Length];
            ret = ext + ret;
        }
        if (ret.StartsWith('.'))
            return ret.Substr(1, ret.Length);
        return ret;
    }

    public static bool HasExtension(this string path, params string[] extensions)
    {
        var pathExt = path.GetFullExtension();
        return extensions.Any(ext => pathExt.Equals(ext, System.StringComparison.CurrentCultureIgnoreCase));
    }
    public static bool StartsWith(this string a, string b, bool caseSensitive) => caseSensitive ? a.StartsWith(b) : a.StartsWith(b, System.StringComparison.CurrentCultureIgnoreCase);
}