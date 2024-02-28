using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

using GodotArray = Godot.Collections.Array;
using GodotDictionary = Godot.Collections.Dictionary;

namespace JSON.Schema;

public class InvalidDataException(IJSONType type, Variant data) : 
    Exception($"Unable to clean JSON using schema {type}. Invalid data provided: {data}") { }

public static class JSONExtensions
{
    public static Variant JSONSerialize(this Vector2 vector) => new GodotDictionary { { "X", vector.X }, { "Y", vector.Y } };
    public static Variant JSONSerialize(this Vector2I vector) => new GodotDictionary { { "X", vector.X }, { "Y", vector.Y } };
    public static Variant JSONSerialize(this Vector3 vector) => new GodotDictionary { { "X", vector.X }, { "Y", vector.Y }, { "Z", vector.Z } };
    public static Variant JSONSerialize(this Vector3I vector) => new GodotDictionary { { "X", vector.X }, { "Y", vector.Y }, { "Z", vector.Z } };
    public static Variant JSONSerialize(this Vector4 vector) => new GodotDictionary { { "X", vector.X }, { "Y", vector.Y }, { "Z", vector.Z }, { "W", vector.W } };
    public static Variant JSONSerialize(this Vector4I vector) => new GodotDictionary { { "X", vector.X }, { "Y", vector.Y }, { "Z", vector.Z }, { "W", vector.W } };
    public static Variant JSONSerialize(this Color color) => new GodotDictionary { { "R", color.R }, { "G", color.G }, { "B", color.B }, { "A", color.A } };
    public static Variant JSONSerialize(this Rect2 rect) => new GodotDictionary
    {
        { "Position", rect.Position.JSONSerialize() },
        { "Size", rect.Size.JSONSerialize() }
    };
    public static Variant JSONSerialize(this Rect2I rect) => new GodotDictionary
    {
        { "Position", rect.Position.JSONSerialize() },
        { "Size", rect.Size.JSONSerialize() }
    };
    public static Variant JSONSerialize(this GodotDictionary dict)
    {
        var newDict = dict.Duplicate();
        foreach (var key in newDict.Keys)
        {
            if (key.TryAs<string>() is null && key.TryAs<StringName>() is null)
                throw new Exception($"Cannot JSON serialize a dictionary with a key of type: {key.GetType()}. Only string or StringName keys are allowed.");
            newDict[key] = newDict[key].JSONSerialize();
        }
        return newDict;
    }
    public static Variant JSONSerialize(this GodotArray array) => array.Select(element => element.JSONSerialize()).ToGodotArray();
    public static Variant JSONSerialize(this IEnumerable<Vector2> array) => Variant.From(array.Select(element => element.JSONSerialize()));
    public static Variant JSONSerialize(this IEnumerable<Vector3> array) => Variant.From(array.Select(element => element.JSONSerialize()));
    public static Variant JSONSerialize(this IEnumerable<Color> array) => Variant.From(array.Select(element => element.JSONSerialize()));
    
    public static Variant JSONSerialize(this Variant value)
    {
        if (value.Is<StringName>() || value.Is<string>() || value.Is<NodePath>()) return value.ToString();
        if (value.Is<int>() || value.Is<bool>() || value.Is<float>() || value.Is<int[]>() || value.Is<float[]>() || value.Is<string[]>() || value.Is<byte[]>()) return value;
        if (value.Is<Vector2>()) return ((Vector2)value).JSONSerialize();
        if (value.Is<Vector2I>()) return ((Vector2I)value).JSONSerialize();
        if (value.Is<Vector3>()) return ((Vector3)value).JSONSerialize();
        if (value.Is<Vector3I>()) return ((Vector3I)value).JSONSerialize();
        if (value.Is<Vector4>()) return ((Vector4)value).JSONSerialize();
        if (value.Is<Vector4I>()) return ((Vector4I)value).JSONSerialize();
        if (value.Is<Color>()) return ((Color)value).JSONSerialize();
        if (value.Is<GodotArray>()) return ((GodotArray)value).JSONSerialize();
        if (value.Is<Vector2[]>()) return ((Vector2[])value).JSONSerialize();
        if (value.Is<Vector3[]>()) return ((Vector3[])value).JSONSerialize();
        if (value.Is<Color[]>()) return ((Color[])value).JSONSerialize();
        if (value.Is<Rect2>()) return ((Rect2)value).JSONSerialize();
        if (value.Is<Rect2I>()) return ((Rect2I)value).JSONSerialize();
        if (value.Is<GodotDictionary>()) return ((GodotDictionary)value).JSONSerialize();
        if (value.Is<GodotObject>()) return new Variant();
        return new Variant();
    }
}

public interface IJSONType
{
    Variant? Default { get; }
    GodotDictionary GetSchema();
    bool IsValid(Variant data);
    Variant Clean(Variant data);
}

internal static class JSONTypeExtensions
{
    public static GodotDictionary WithDefault<T, [MustBeVariant] BaseType>(this T prop, GodotDictionary other) where T : IJSONType
    {
        if (prop.Default?.Is<BaseType>() ?? false) other["default"] = ((Variant)prop.Default!).JSONSerialize();
        return other;
    }
}

public class Generator : IIdentifiable<string>
{
    private readonly string SchemaURI = "https://json-schema.org/draft/2020-12/schema";
    
    private static readonly Variant.Type[] ExcludeTypes = [
        Variant.Type.Nil
    ];

    private static readonly PropertyUsageFlags AlwaysExcludeUsageFlags =
        PropertyUsageFlags.Group
        | PropertyUsageFlags.Subgroup
        | PropertyUsageFlags.Category
        | PropertyUsageFlags.Secret
        | PropertyUsageFlags.Internal
        | PropertyUsageFlags.Subgroup;

    public required StringName ClassName;
    public required string Path;
    public Script? Script;
    public string? Title = null;
    public readonly string? Description = null;
    public StringName[] RequiredProperties = [];
    public StringName[] ExcludeProperties = [];
    public PropertyUsageFlags ExcludeUsageFlags = PropertyUsageFlags.None;
    public bool ConvertCase = false;
    
    public string ID => Path.Replace("res://", "").Replace(".schema.json", "");
    
    public void Generate()
    {
        // Validate path
        if (!Path.StartsWith("res://")) throw new Exception($"Expected \"res://\" path, found \"{Path}\".");
        
        GodotDictionary dict = new();
        
        // Add metadata
        dict["$schema"] = SchemaURI;
        dict["$id"] = ID;
        if (Title is not null) dict["title"] = Title;
        if (Description is not null) dict["description"] = Description;
        
        // Add properties
        dict.Merge(GetSchema().GetSchema());
        
        // Create directory, if needed
        if (!DirAccess.DirExistsAbsolute(Path.GetBaseDir())) DirAccess.MakeDirRecursiveAbsolute(Path.GetBaseDir());
        
        // Store JSON
        var file = FileAccess.Open(Path, FileAccess.ModeFlags.Write);
        file.StoreString(Json.Stringify(dict, "  ", sortKeys: false));
        file.Close();
        GD.Print($"Successfully generated schema for \"{ClassName}\" at \"{Path}\".");
    }

    public GodotDictionary Clean(Variant data) => GetSchema().Clean(data).TryAs<GodotDictionary>() ?? throw new InvalidDataException(GetSchema(), data);

    private Type.JSON GetSchema()
    {
        var cls = new GodotClass(ClassName, Script);
        var props = cls.GetProperties()
            .ExceptBy(ExcludeProperties, prop => prop.Name)
            .Where(prop => !ExcludeTypes.Contains(prop.Ty))
            .Where(prop => (uint)(prop.Usage & AlwaysExcludeUsageFlags) != 1)
            .Where(prop => (uint)(prop.Usage & ExcludeUsageFlags) != 1);
        var propNames = props.Select(prop => prop.Name ?? "");
        var namedProps = propNames.Zip(props.Select(Type.JSON.FromProperty));
        return new Type.JSON
        {
            Properties = namedProps.ToDictionary(), 
            RequiredProperties = RequiredProperties, 
            ConvertCase = ConvertCase,
            AdditionalProperties = false,
        };
    }
}

public sealed class GeneratorDB : SingletonDB<Generator>
{
    public static void Generate() => Resources.ForEach(resource => resource.Generate());
    public static Generator? Find(Script? script) => Resources.FirstOrDefault(resource => resource.Script == script && script is not null);
    public static Generator? Find(StringName? name) => Resources.FirstOrDefault(resource => resource.ClassName == name && !name.IsEmpty());
}