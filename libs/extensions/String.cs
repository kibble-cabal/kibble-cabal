using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Godot;

public static class StringExtensions
{
    private static Dictionary<Regex, string> PascalReplacements = new()
    {
        { new Regex("Ui(?=$|[A-Z])"), "UI" },
        { new Regex("Zip(?=$|[A-Z])"), "ZIP" },
        { new Regex("Json(?=$|[A-Z])"), "JSON" },
        { new Regex("Id(?=$|[A-Z])"), "ID" }
    };
    
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

    public static bool IsEmpty(this string str) => string.IsNullOrEmpty(str);
    public static bool IsEmpty(this StringName str) => string.IsNullOrEmpty(str);

    public static string ReplaceMany(this string str, Dictionary<Regex, string> replacements)
    {
        var s = str;
        foreach (var key in replacements.Keys)
            s = key.Replace(s, replacements[key]);
        return s;
    }
    
    public static string Pascal(this string str) => str.ToPascalCase().ReplaceMany(PascalReplacements);
    public static StringName Pascal(this StringName str) => ((string)str).Pascal();
}