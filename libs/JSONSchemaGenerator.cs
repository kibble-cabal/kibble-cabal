using System;
using System.Collections.Generic;
using System.Linq;
using BB;
using Godot;
using Godot.Collections;

using GodotArray = Godot.Collections.Array;

namespace JSONSchema
{
    internal class GodotClass
    {
        private readonly StringName? ClassName;
        private Script? Script;

        public GodotClass(StringName? name = null, Script? script = null)
        {
            ClassName = name;
            Script = script;
            GetScriptFromClassName();
        }

        private Dictionary? GetGlobalClass() => ProjectSettings
            .GetGlobalClassList()
            .FirstOrDefault(cls => cls["class"].As<StringName>() == ClassName && !ClassName.IsEmpty());

        private bool IsBuiltinClass() => ClassDB.ClassExists(ClassName);
        private bool IsGlobalClass() => GetGlobalClass() is not null;

        private void GetScriptFromClassName()
        {
            if (ClassName is not null
                && Script is null
                && !ClassName.IsEmpty()
                && !ClassDB.ClassExists(ClassName) 
                && GetGlobalClass() is { } classDict)
                Script = GD.Load<Script>(classDict["path"].AsString());
        }

        public IEnumerable<Property> GetProperties()
        {
            if (Script is not null) return Script.GetScriptPropertyList().Select(prop =>
            {
                prop["default"] = Script.GetPropertyDefaultValue(prop["name"].As<StringName>());
                return prop;
            }).Select(Property.From);
            if (ClassName is not null) return ClassDB.ClassGetPropertyList(ClassName).Select(Property.From);
            throw new Exception($"Class could not be found. (\"{ClassName}\", {Script})");
        }

        public Generator? FindGenerator()
        {
            if (IsGlobalClass()) return GeneratorDB.Find(ClassName);
            if (Script is not null) return GeneratorDB.Find(Script);
            if (IsBuiltinClass()) return GeneratorDB.Find(ClassName);
            return null;
        }

        public override string ToString()
        {
            if (Script is not null && ClassName is not null) return $"Class({ClassName}, \"{Script.ResourcePath}\")";
            if (Script is not null) return $"Class(\"{Script.ResourcePath}\")";
            return $"Class({ClassName})";
        }
    }
    
    internal static class JSONExtensions
    {
        public static Variant JSONSerialize(this Vector2 vector) => new Dictionary { { "X", vector.X }, { "Y", vector.Y } };
        public static Variant JSONSerialize(this Vector2I vector) => new Dictionary { { "X", vector.X }, { "Y", vector.Y } };
        public static Variant JSONSerialize(this Vector3 vector) => new Dictionary { { "X", vector.X }, { "Y", vector.Y }, { "Z", vector.Z } };
        public static Variant JSONSerialize(this Vector3I vector) => new Dictionary { { "X", vector.X }, { "Y", vector.Y }, { "Z", vector.Z } };
        public static Variant JSONSerialize(this Vector4 vector) => new Dictionary { { "X", vector.X }, { "Y", vector.Y }, { "Z", vector.Z }, { "W", vector.W } };
        public static Variant JSONSerialize(this Vector4I vector) => new Dictionary { { "X", vector.X }, { "Y", vector.Y }, { "Z", vector.Z }, { "W", vector.W } };
        public static Variant JSONSerialize(this Color color) => new Dictionary { { "R", color.R }, { "G", color.G }, { "B", color.B }, { "A", color.A } };
        public static Variant JSONSerialize(this Rect2 rect) => new Dictionary
        {
            { "Position", rect.Position.JSONSerialize() },
            { "Size", rect.Size.JSONSerialize() }
        };
        public static Variant JSONSerialize(this Rect2I rect) => new Dictionary
        {
            { "Position", rect.Position.JSONSerialize() },
            { "Size", rect.Size.JSONSerialize() }
        };
        public static Variant JSONSerialize(this Dictionary dict)
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
            if (value.Is<Dictionary>()) return ((Dictionary)value).JSONSerialize();
            if (value.Is<GodotObject>()) return new Variant();
            return new Variant();
        }
    }

    internal interface IJSONProperty
    {
        Variant? Default { get; }
        Dictionary ToDict();
    }

    internal static class JSONPropertyExtensions
    {
        public static Dictionary WithDefault<T, [MustBeVariant] BaseType>(this T prop, Dictionary other) where T : IJSONProperty
        {
            if (prop.Default?.Is<BaseType>() ?? false) other["default"] = ((Variant)prop.Default!).JSONSerialize();
            return other;
        }
    }

    internal class Boolean : IJSONProperty
    {
        public Variant? Default { get; set; }
        public static Boolean FromProperty(Property property) => new() { Default = property.Default };
        public Dictionary ToDict() => this.WithDefault<Boolean, bool>(new Dictionary { { "type", "boolean" } });
    }

    internal class Number : IJSONProperty
    {
        private float? Min;
        private float? Max;
        private float? Step;
        public Variant? Default { get; set; }
        public static Number FromProperty(Property property)
        {
            var strs = property.HintString.Split(",");
            var min = strs.Has(0) && !property.HintString.Contains("or_less") ? strs[0].TryParseFloat() : null;
            var max = strs.Has(1) && !property.HintString.Contains("or_greater") ? strs[1].TryParseFloat() : null;
            var step = strs.Has(2) ? strs[2].TryParseFloat() : null;
            return new Number { Min = min, Max = max, Step = step, Default = property.Default };
        }
        public Dictionary ToDict()
        {
            var dict = new Dictionary { { "type", "number" } };
            if (Min is { } min) dict["minimum"] = min;
            if (Max is { } max) dict["maximum"] = max;
            if (Step is { } step) dict["step"] = step;
            return this.WithDefault<Number, float>(dict);
        }
    }

    internal class Integer : IJSONProperty
    {
        private int? Min;
        private int? Max;
        private int? Step;
        public Variant? Default { get; set; }
        public static Integer FromProperty(Property property)
        {
            var strs = property.HintString.Split(",");
            var min = strs.Has(0) && !property.HintString.Contains("or_less") ? strs[0].TryParseInt() : null;
            var max = strs.Has(1) && !property.HintString.Contains("or_greater") ? strs[1].TryParseInt() : null;
            var step = strs.Has(2) ? strs[2].TryParseInt() : null;
            return new Integer { Min = min, Max = max, Step = step, Default = property.Default };
        }
        public Dictionary ToDict()
        {
            var dict = new Dictionary { { "type", "number" } };
            if (Min is { } min) dict["minimum"] = min;
            if (Max is { } max) dict["maximum"] = max;
            if (Step is { } step) dict["step"] = step;
            return this.WithDefault<Integer, int>(dict);
        }
    }

    internal class String : IJSONProperty
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
        public Dictionary ToDict()
        {
            var dict = new Dictionary { { "type", "string" } };
            if (IsFile || IsDir) dict["format"] = "uri-reference";
            if (IsFile) dict["description"] = "A file path.";
            if (IsDir) dict["description"] = "A directory path.";
            return this.WithDefault<String, string>(dict);
        }
    }

    internal class Dict : IJSONProperty
    {
        public Variant? Default { get; set; }
        public static Dict FromProperty(Property property) => new() { Default = property.Default };
        public Dictionary ToDict() => this.WithDefault<Dict, Dictionary>(new Dictionary { { "type", "object" }, { "additionalProperties", true } });
    }

    internal class GodotResource : IJSONProperty
    {
        private GodotClass? GodotClass;
        public Variant? Default { get; set; }

        public static GodotResource FromProperty(Property property)
        {
            var obj = new GodotResource(); // TODO default
            if (property.Hint is PropertyHint.ResourceType)
                obj.GodotClass = new GodotClass(property.HintString);
            return obj;
        }

        public Dictionary ToDict()
        {
            var generator = GodotClass?.FindGenerator();
            if (generator is not null)
                return new Dictionary { { "$ref", generator.ID } }; // TODO default
            GD.PrintRich($"[Warning] Cannot reference Godot class {GodotClass} in schema.".Yellow());
            return new Dictionary { { "$ref", "unknown" } };
        }
    }

    internal class Array(IJSONProperty? elementSchema = null, Variant? defaultValue = null) : IJSONProperty
    {
        public readonly IJSONProperty? ElementSchema = elementSchema;
        public Variant? Default { get; set; } = defaultValue;

        public static Array FromProperty(Property property)
        {
            if (property.Ty != Variant.Type.Array) return property.Ty switch
            {
                Variant.Type.PackedByteArray
                    or Variant.Type.PackedInt32Array
                    or Variant.Type.PackedInt64Array => new Array(new Integer(), property.Default),
                Variant.Type.PackedFloat32Array
                    or Variant.Type.PackedFloat64Array => new Array(new Number(), property.Default),
                Variant.Type.PackedStringArray => new Array(new String(), property.Default),
                Variant.Type.PackedVector2Array => new Array(new Vector(new Number(), "X", "Y")),
                Variant.Type.PackedVector3Array => new Array(new Vector(new Number(), "X", "Y", "Z")),
                Variant.Type.PackedColorArray when property.Hint == PropertyHint.ColorNoAlpha => new Array(new Vector(new Number(), "R", "G", "B")),
                Variant.Type.PackedColorArray => new Array(new Vector(new Number(), "R", "G", "B", "A")),
                _ => throw new NotImplementedException($"Not implemented: Array.FromProperty({property.Name})")
            };
            var strings = property.HintString.Split(["/", ":"], StringSplitOptions.TrimEntries & StringSplitOptions.RemoveEmptyEntries);
            if (property.Hint is PropertyHint.ArrayType or PropertyHint.TypeString && strings.Length > 0)
            {
                var type = (Variant.Type)(strings[0].TryParseInt() ?? 0);
                var nextHint = strings.SelectElementAt(1, val => (PropertyHint)(val.TryParseInt() ?? 0));
                var nextHintString = strings.ElementAtOr(2, "");
                var nextProperty = JSON.FromProperty(new Property(name: "element", type, nextHint, nextHintString));
                return new Array(nextProperty, property.Default);
            }
            return new Array(null, property.Default);
        }

        public Dictionary ToDict()
        {
            var dict = new Dictionary { { "type", "array" } };
            if (ElementSchema is not null) dict["items"] = ElementSchema.ToDict();
            return this.WithDefault<Array, GodotArray>(dict);
        }
    }

    internal class JSON : IJSONProperty
    {
        public System.Collections.Generic.Dictionary<StringName, IJSONProperty> Properties = [];
        public StringName[] RequiredProperties = [];
        public bool AdditionalProperties = true;
        public Variant? Default { get; set; }
        public bool ConvertCase = true;

        public static IJSONProperty FromProperty(Property property)
        {
            return property.Ty switch
            {
                Variant.Type.Array
                    or Variant.Type.PackedByteArray
                    or Variant.Type.PackedInt32Array
                    or Variant.Type.PackedInt64Array
                    or Variant.Type.PackedFloat32Array
                    or Variant.Type.PackedFloat64Array
                    or Variant.Type.PackedStringArray
                    or Variant.Type.PackedColorArray
                    or Variant.Type.PackedVector2Array
                    or Variant.Type.PackedVector3Array => Array.FromProperty(property),
                Variant.Type.Bool => Boolean.FromProperty(property),
                Variant.Type.Dictionary => Dict.FromProperty(property),
                Variant.Type.Float => Number.FromProperty(property),
                Variant.Type.Int => Integer.FromProperty(property),
                Variant.Type.String or Variant.Type.StringName => String.FromProperty(property),
                Variant.Type.Vector2
                    or Variant.Type.Vector2I
                    or Variant.Type.Vector3
                    or Variant.Type.Vector3I
                    or Variant.Type.Vector4
                    or Variant.Type.Vector4I
                    or Variant.Type.Color => Vector.FromProperty(property),
                Variant.Type.Rect2 or Variant.Type.Rect2I => Rect.FromProperty(property),
                Variant.Type.Object => GodotResource.FromProperty(property),
                _ => throw new NotImplementedException($"Not implemented: {property.Name} ({property.Ty})")
            };
        }

        public Dictionary ToDict()
        {
            var propDict = new Dictionary();
            foreach (var propName in Properties.Keys)
                propDict[ConvertCase ? propName.Pascal() : propName] = Properties[propName].ToDict();
            var dict = new Dictionary
            {
                { "type", "object" },
                { "properties", propDict },
                { "required", RequiredProperties },
                { "additionalProperties", AdditionalProperties }
            };
            return dict;
        }
    }

    internal class Vector : JSON
    {
        public Vector(IJSONProperty elementSchema, Variant? defaultValue = null, params StringName[] elementNames)
        {
            Properties = elementNames.Select(name => (name, elementSchema)).ToDictionary();
            RequiredProperties = elementNames;
            Default = defaultValue?.JSONSerialize();
            AdditionalProperties = false;
        }
        public static Vector Vector2(Variant? defaultValue = null) => new(new Number(), defaultValue, "X", "Y");
        public static Vector Vector2I(Variant? defaultValue = null) => new(new Integer(), defaultValue, "X", "Y");
        public static Vector Vector3(Variant? defaultValue = null) => new(new Number(), defaultValue, "X", "Y", "Z");
        public static Vector Vector3I(Variant? defaultValue = null) => new(new Integer(), defaultValue, "X", "Y", "Z");
        public static Vector Vector4(Variant? defaultValue = null) => new(new Number(), defaultValue, "X", "Y", "Z", "W");
        public static Vector Vector4I(Variant? defaultValue = null) => new(new Integer(), defaultValue, "X", "Y", "Z", "W");
        public static Vector ColorNoAlpha(Variant? defaultValue = null) => new(new Number(), defaultValue, "R", "G", "B");
        public static Vector Color(Variant? defaultValue = null) => new(new Number(), defaultValue, "R", "G", "B", "A");
        public new static Vector FromProperty(Property property) => property.Ty switch
        {
            Variant.Type.Vector2 => Vector2(property.Default),
            Variant.Type.Vector2I => Vector2I(property.Default),
            Variant.Type.Vector3 => Vector3(property.Default),
            Variant.Type.Vector3I => Vector3I(property.Default),
            Variant.Type.Vector4 => Vector4(property.Default),
            Variant.Type.Vector4I => Vector4I(property.Default),
            Variant.Type.Color when property.Hint == PropertyHint.ColorNoAlpha => ColorNoAlpha(property.Default),
            Variant.Type.Color => Color(property.Default),
            _ => throw new NotImplementedException($"Not implemented: Vector.FromProperty({property.Name})")
        };
        public new Dictionary ToDict() => this.WithDefault<Vector, Dictionary>(base.ToDict());
    }

    internal class Rect : JSON
    {
        public Rect(IJSONProperty elementSchema, Variant? defaultValue = null)
        {
            StringName[] propertyNames = ["Size", "Position"];
            Properties = propertyNames.Select(name => (name, elementSchema)).ToDictionary();
            RequiredProperties = propertyNames;
            Default = defaultValue?.JSONSerialize();
            AdditionalProperties = false;
        }
        public new static Rect FromProperty(Property property) => property.Ty switch
        {
            Variant.Type.Rect2 => new Rect(Vector.Vector2()),
            Variant.Type.Rect2I => new Rect(Vector.Vector2I()),
            _ => throw new NotImplementedException($"Not implemented: Rect.FromProperty({property.Name})")
        };
        public new Dictionary ToDict() => this.WithDefault<Rect, Dictionary>(base.ToDict());
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
            
            Dictionary dict = new();
            
            // Add metadata
            dict["$schema"] = SchemaURI;
            dict["$id"] = ID;
            if (Title is not null) dict["title"] = Title;
            if (Description is not null) dict["description"] = Description;
            
            // Add properties
            dict.Merge(GetSchema().ToDict());
            
            // Create directory, if needed
            if (!DirAccess.DirExistsAbsolute(Path.GetBaseDir())) DirAccess.MakeDirRecursiveAbsolute(Path.GetBaseDir());
            
            // Store JSON
            var file = FileAccess.Open(Path, FileAccess.ModeFlags.Write);
            file.StoreString(Json.Stringify(dict, "  ", sortKeys: false));
            file.Close();
            GD.Print($"Successfully generated schema for \"{ClassName}\" at \"{Path}\".");
        }

        private JSON GetSchema()
        {
            var cls = new GodotClass(ClassName, Script);
            var props = cls.GetProperties()
                .ExceptBy(ExcludeProperties, prop => prop.Name)
                .Where(prop => !ExcludeTypes.Contains(prop.Ty))
                .Where(prop => (uint)(prop.Usage & AlwaysExcludeUsageFlags) != 1)
                .Where(prop => (uint)(prop.Usage & ExcludeUsageFlags) != 1);
            var propNames = props.Select(prop => prop.Name ?? "");
            var namedProps = propNames.Zip(props.Select(JSON.FromProperty));
            return new JSON
            {
                Properties = namedProps.ToDictionary(), 
                RequiredProperties = RequiredProperties, 
                ConvertCase = ConvertCase
            };
        }
    }

    public sealed class GeneratorDB : SingletonDB<Generator>
    {
        public static void Generate() => Resources.ForEach(resource => resource.Generate());
        public static Generator? Find(Script? script) => Resources.FirstOrDefault(resource => resource.Script == script && script is not null);
        public static Generator? Find(StringName? name) => Resources.FirstOrDefault(resource => resource.ClassName == name && !name.IsEmpty());
    }
}