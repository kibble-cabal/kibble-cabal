using Godot;

using GodotDictionary = Godot.Collections.Dictionary;

namespace JSON.Schema.Type;

public class String : IJSONType
{
    private bool IsFile;
    private bool IsDir;
    public Variant? Default { get; set; }
    public static String FromProperty(Property property) => new()
    {
        IsFile = property.Hint is PropertyHint.File or PropertyHint.GlobalFile,
        IsDir = property.Hint is PropertyHint.Dir or PropertyHint.GlobalDir,
        Default = property.Default
    };
    public GodotDictionary GetSchema()
    {
        var dict = new GodotDictionary { { "type", "string" } };
        if (IsFile || IsDir) dict["format"] = "uri-reference";
        if (IsFile) dict["description"] = "A file path.";
        if (IsDir) dict["description"] = "A directory path.";
        return this.WithDefault<String, string>(dict);
    }

    public bool IsValid(Variant data) => data.Obj is string or StringName or NodePath;
    public Variant Clean(Variant data) => IsValid(data) ? data : throw new InvalidDataException(this, data);
}