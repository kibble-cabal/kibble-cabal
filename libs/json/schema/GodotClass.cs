using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

using GodotDictionary = Godot.Collections.Dictionary;

namespace JSON.Schema;

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

    private GodotDictionary? GetGlobalClass() => ProjectSettings
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