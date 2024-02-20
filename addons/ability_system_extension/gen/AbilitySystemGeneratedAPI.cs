/// 0.1.0-alpha
/// ////////////////////////////////////////////////
/// THIS FILE HAS BEEN GENERATED.
/// THE CHANGES IN THIS FILE WILL BE OVERWRITTEN
/// AFTER THE UPDATE OR AFTER THE RESTART!
/// ////////////////////////////////////////////////

using Godot;
using System;
using System.Linq;
#nullable disable

public partial class Ability : _AbilitySystemInstanceWrapper_<Resource>
{
    public Ability(Resource _instance) : base (_instance) {}
    
    public Ability() : this((Resource)ClassDB.Instantiate("Ability")) { }
    
    public static new class Enum
    {
        public enum Mode : long
        {
            Parallel = 1,
            Sequential = 2,
        }
    }
    
    public static new class MethodName
    {
    }
    
    public static new class PropertyName
    {
        public static readonly StringName Identifier = "identifier";
        public static readonly StringName TagsBlocking = "tags_blocking";
        public static readonly StringName TagsRequired = "tags_required";
        public static readonly StringName Effects = "effects";
        public static readonly StringName EffectMode = "effect_mode";
        public static readonly StringName UIColor = "ui_color";
    }
    
    public StringName Identifier
    {
        get => (StringName)ClassDB.ClassGetProperty(Instance, PropertyName.Identifier);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.Identifier, value);
    }
    
    public Godot.Collections.Array TagsBlocking
    {
        get => (Godot.Collections.Array)ClassDB.ClassGetProperty(Instance, PropertyName.TagsBlocking);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.TagsBlocking, value);
    }
    
    public Godot.Collections.Array TagsRequired
    {
        get => (Godot.Collections.Array)ClassDB.ClassGetProperty(Instance, PropertyName.TagsRequired);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.TagsRequired, value);
    }
    
    public Godot.Collections.Array Effects
    {
        get => (Godot.Collections.Array)ClassDB.ClassGetProperty(Instance, PropertyName.Effects);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.Effects, value);
    }
    
    public int EffectMode
    {
        get => (int)ClassDB.ClassGetProperty(Instance, PropertyName.EffectMode);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.EffectMode, value);
    }
    
    public Color UIColor
    {
        get => (Color)ClassDB.ClassGetProperty(Instance, PropertyName.UIColor);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.UIColor, value);
    }
    
}

public partial class AbilityEvent : _AbilitySystemInstanceWrapper_<Resource>
{
    public AbilityEvent(Resource _instance) : base (_instance) {}
    
    public AbilityEvent() : this((Resource)ClassDB.Instantiate("AbilityEvent")) { }
    
    public static new class MethodName
    {
    }
    
    public static new class PropertyName
    {
        public static readonly StringName Ability = "ability";
        public static readonly StringName EffectInstances = "effect_instances";
    }
    
    public Ability Ability
    {
        get => (Ability)_AbilitySystemUtils_.CreateWrapperFromObject((GodotObject)ClassDB.ClassGetProperty(Instance, PropertyName.Ability));
        set => ClassDB.ClassSetProperty(Instance, PropertyName.Ability, value.Instance);
    }
    
    public Godot.Collections.Array EffectInstances
    {
        get => (Godot.Collections.Array)ClassDB.ClassGetProperty(Instance, PropertyName.EffectInstances);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.EffectInstances, value);
    }
    
}

public partial class AbilitySystem : _AbilitySystemInstanceWrapper_<Node>
{
    public AbilitySystem(Node _instance) : base (_instance) {}
    
    public AbilitySystem() : this((Node)ClassDB.Instantiate("AbilitySystem")) { }
    
    public static new class Enum
    {
        public enum UpdateMode : long
        {
            Disabled = 0,
            Physics = 1,
            Process = 2,
        }
    }
    
    public static new class MethodName
    {
        public static readonly StringName HasAttribute = "has_attribute";
        public static readonly StringName GrantAttribute = "grant_attribute";
        public static readonly StringName RevokeAttribute = "revoke_attribute";
        public static readonly StringName GetAttributeValue = "get_attribute_value";
        public static readonly StringName SetAttributeValue = "set_attribute_value";
        public static readonly StringName ModifyAttributeValue = "modify_attribute_value";
        public static readonly StringName CanActivate = "can_activate";
        public static readonly StringName HasAbility = "has_ability";
        public static readonly StringName GrantAbility = "grant_ability";
        public static readonly StringName RevokeAbility = "revoke_ability";
        public static readonly StringName Activate = "activate";
        public static readonly StringName HasTag = "has_tag";
        public static readonly StringName HasSomeTags = "has_some_tags";
        public static readonly StringName HasAllTags = "has_all_tags";
        public static readonly StringName GrantTag = "grant_tag";
        public static readonly StringName RevokeTag = "revoke_tag";
    }
    
    
    public bool HasAttribute(Attribute attribute) => (bool)(Instance?.Call(MethodName.HasAttribute, attribute.Instance));
    
    public void GrantAttribute(Attribute attribute) => Instance?.Call(MethodName.GrantAttribute, attribute.Instance);
    
    public void RevokeAttribute(Attribute attribute) => Instance?.Call(MethodName.RevokeAttribute, attribute.Instance);
    
    public float GetAttributeValue(Attribute attribute) => (float)(Instance?.Call(MethodName.GetAttributeValue, attribute.Instance));
    
    public void SetAttributeValue(Attribute attribute, float value) => Instance?.Call(MethodName.SetAttributeValue, attribute.Instance, value);
    
    public void ModifyAttributeValue(Attribute attribute, float by_amount) => Instance?.Call(MethodName.ModifyAttributeValue, attribute.Instance, by_amount);
    
    public bool CanActivate(Ability ability) => (bool)(Instance?.Call(MethodName.CanActivate, ability.Instance));
    
    public bool HasAbility(Ability ability) => (bool)(Instance?.Call(MethodName.HasAbility, ability.Instance));
    
    public void GrantAbility(Ability ability) => Instance?.Call(MethodName.GrantAbility, ability.Instance);
    
    public void RevokeAbility(Ability ability) => Instance?.Call(MethodName.RevokeAbility, ability.Instance);
    
    public AbilityEvent Activate(Ability ability) => (AbilityEvent)_AbilitySystemUtils_.CreateWrapperFromObject((GodotObject)Instance?.Call(MethodName.Activate, ability.Instance));
    
    public bool HasTag(Tag tag) => (bool)(Instance?.Call(MethodName.HasTag, tag.Instance));
    
    public bool HasSomeTags(Godot.Collections.Array tags) => (bool)(Instance?.Call(MethodName.HasSomeTags, tags));
    
    public bool HasAllTags(Godot.Collections.Array tags) => (bool)(Instance?.Call(MethodName.HasAllTags, tags));
    
    public void GrantTag(Tag tag) => Instance?.Call(MethodName.GrantTag, tag.Instance);
    
    public void RevokeTag(Tag tag) => Instance?.Call(MethodName.RevokeTag, tag.Instance);
    
    public static new class PropertyName
    {
        public static readonly StringName Tags = "tags";
        public static readonly StringName Abilities = "abilities";
        public static readonly StringName Events = "events";
        public static readonly StringName Attributes = "attributes";
        public static readonly StringName UpdateMode = "update_mode";
    }
    
    public Godot.Collections.Array Tags
    {
        get => (Godot.Collections.Array)ClassDB.ClassGetProperty(Instance, PropertyName.Tags);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.Tags, value);
    }
    
    public Godot.Collections.Array Abilities
    {
        get => (Godot.Collections.Array)ClassDB.ClassGetProperty(Instance, PropertyName.Abilities);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.Abilities, value);
    }
    
    public Godot.Collections.Array Events
    {
        get => (Godot.Collections.Array)ClassDB.ClassGetProperty(Instance, PropertyName.Events);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.Events, value);
    }
    
    public Godot.Collections.Dictionary Attributes
    {
        get => (Godot.Collections.Dictionary)ClassDB.ClassGetProperty(Instance, PropertyName.Attributes);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.Attributes, value);
    }
    
    public int UpdateMode
    {
        get => (int)ClassDB.ClassGetProperty(Instance, PropertyName.UpdateMode);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.UpdateMode, value);
    }
    
}

public partial class Attribute : _AbilitySystemInstanceWrapper_<Resource>
{
    public Attribute(Resource _instance) : base (_instance) {}
    
    public Attribute() : this((Resource)ClassDB.Instantiate("Attribute")) { }
    
    public static new class MethodName
    {
    }
    
    public static new class PropertyName
    {
        public static readonly StringName Identifier = "identifier";
        public static readonly StringName MaxValue = "max_value";
        public static readonly StringName MinValue = "min_value";
        public static readonly StringName DefaultValue = "default_value";
        public static readonly StringName UIColor = "ui_color";
    }
    
    public StringName Identifier
    {
        get => (StringName)ClassDB.ClassGetProperty(Instance, PropertyName.Identifier);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.Identifier, value);
    }
    
    public float MaxValue
    {
        get => (float)ClassDB.ClassGetProperty(Instance, PropertyName.MaxValue);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.MaxValue, value);
    }
    
    public float MinValue
    {
        get => (float)ClassDB.ClassGetProperty(Instance, PropertyName.MinValue);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.MinValue, value);
    }
    
    public float DefaultValue
    {
        get => (float)ClassDB.ClassGetProperty(Instance, PropertyName.DefaultValue);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.DefaultValue, value);
    }
    
    public Color UIColor
    {
        get => (Color)ClassDB.ClassGetProperty(Instance, PropertyName.UIColor);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.UIColor, value);
    }
    
}

public partial class Tag : _AbilitySystemInstanceWrapper_<Resource>
{
    public Tag(Resource _instance) : base (_instance) {}
    
    public Tag() : this((Resource)ClassDB.Instantiate("Tag")) { }
    
    public static new class MethodName
    {
    }
    
    public static new class PropertyName
    {
        public static readonly StringName Identifier = "identifier";
        public static readonly StringName UIColor = "ui_color";
    }
    
    public StringName Identifier
    {
        get => (StringName)ClassDB.ClassGetProperty(Instance, PropertyName.Identifier);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.Identifier, value);
    }
    
    public Color UIColor
    {
        get => (Color)ClassDB.ClassGetProperty(Instance, PropertyName.UIColor);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.UIColor, value);
    }
    
}

public partial class Effect : _AbilitySystemInstanceWrapper_<Resource>
{
    public Effect(Resource _instance) : base (_instance) {}
    
    public Effect() : this((Resource)ClassDB.Instantiate("Effect")) { }
    
    public static new class Enum
    {
        public enum Status : long
        {
            Ready = 0,
            Running = 1,
            Finished = 2,
        }
    }
    
    public static new class MethodName
    {
    }
    
    public static new class PropertyName
    {
        public static readonly StringName ElapsedTime = "elapsed_time";
        public static readonly StringName UIName = "ui_name";
        public static readonly StringName UIColor = "ui_color";
    }
    
    public float ElapsedTime
    {
        get => (float)ClassDB.ClassGetProperty(Instance, PropertyName.ElapsedTime);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.ElapsedTime, value);
    }
    
    public StringName UIName
    {
        get => (StringName)ClassDB.ClassGetProperty(Instance, PropertyName.UIName);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.UIName, value);
    }
    
    public Color UIColor
    {
        get => (Color)ClassDB.ClassGetProperty(Instance, PropertyName.UIColor);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.UIColor, value);
    }
    
}

public partial class AttributeEffect : Effect
{
    public AttributeEffect(Resource _instance) : base (_instance) {}
    
    public AttributeEffect() : this((Resource)ClassDB.Instantiate("AttributeEffect")) { }
    
    public static new class MethodName
    {
    }
    
    public static new class PropertyName
    {
        public static readonly StringName Attribute = "attribute";
        public static readonly StringName MinEffect = "min_effect";
        public static readonly StringName MaxEffect = "max_effect";
    }
    
    public Attribute Attribute
    {
        get => (Attribute)_AbilitySystemUtils_.CreateWrapperFromObject((GodotObject)ClassDB.ClassGetProperty(Instance, PropertyName.Attribute));
        set => ClassDB.ClassSetProperty(Instance, PropertyName.Attribute, value.Instance);
    }
    
    public float MinEffect
    {
        get => (float)ClassDB.ClassGetProperty(Instance, PropertyName.MinEffect);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.MinEffect, value);
    }
    
    public float MaxEffect
    {
        get => (float)ClassDB.ClassGetProperty(Instance, PropertyName.MaxEffect);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.MaxEffect, value);
    }
    
}

public partial class LoopEffect : Effect
{
    public LoopEffect(Resource _instance) : base (_instance) {}
    
    public LoopEffect() : this((Resource)ClassDB.Instantiate("LoopEffect")) { }
    
    public static new class MethodName
    {
    }
    
    public static new class PropertyName
    {
        public static readonly StringName MaxLoops = "max_loops";
        public static readonly StringName ElapsedLoops = "elapsed_loops";
    }
    
    public int MaxLoops
    {
        get => (int)ClassDB.ClassGetProperty(Instance, PropertyName.MaxLoops);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.MaxLoops, value);
    }
    
    public int ElapsedLoops
    {
        get => (int)ClassDB.ClassGetProperty(Instance, PropertyName.ElapsedLoops);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.ElapsedLoops, value);
    }
    
}

public partial class TagEffect : Effect
{
    public TagEffect(Resource _instance) : base (_instance) {}
    
    public TagEffect() : this((Resource)ClassDB.Instantiate("TagEffect")) { }
    
    public static new class Enum
    {
        public enum OperationType : long
        {
            Add = 1,
            Remove = -1,
        }
    }
    
    public static new class MethodName
    {
    }
    
    public static new class PropertyName
    {
        public static readonly StringName Tags = "tags";
        public static readonly StringName Operation = "operation";
    }
    
    public Godot.Collections.Array Tags
    {
        get => (Godot.Collections.Array)ClassDB.ClassGetProperty(Instance, PropertyName.Tags);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.Tags, value);
    }
    
    public int Operation
    {
        get => (int)ClassDB.ClassGetProperty(Instance, PropertyName.Operation);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.Operation, value);
    }
    
}

public partial class TryActivateAbilityEffect : Effect
{
    public TryActivateAbilityEffect(Resource _instance) : base (_instance) {}
    
    public TryActivateAbilityEffect() : this((Resource)ClassDB.Instantiate("TryActivateAbilityEffect")) { }
    
    public static new class MethodName
    {
    }
    
    public static new class PropertyName
    {
        public static readonly StringName Ability = "ability";
    }
    
    public Ability Ability
    {
        get => (Ability)_AbilitySystemUtils_.CreateWrapperFromObject((GodotObject)ClassDB.ClassGetProperty(Instance, PropertyName.Ability));
        set => ClassDB.ClassSetProperty(Instance, PropertyName.Ability, value.Instance);
    }
    
}

public partial class WaitEffect : Effect
{
    public WaitEffect(Resource _instance) : base (_instance) {}
    
    public WaitEffect() : this((Resource)ClassDB.Instantiate("WaitEffect")) { }
    
    public static new class MethodName
    {
    }
    
    public static new class PropertyName
    {
        public static readonly StringName MinWaitTime = "min_wait_time";
        public static readonly StringName MaxWaitTime = "max_wait_time";
        public static readonly StringName WaitTime = "wait_time";
    }
    
    public float MinWaitTime
    {
        get => (float)ClassDB.ClassGetProperty(Instance, PropertyName.MinWaitTime);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.MinWaitTime, value);
    }
    
    public float MaxWaitTime
    {
        get => (float)ClassDB.ClassGetProperty(Instance, PropertyName.MaxWaitTime);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.MaxWaitTime, value);
    }
    
    public float WaitTime
    {
        get => (float)ClassDB.ClassGetProperty(Instance, PropertyName.WaitTime);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.WaitTime, value);
    }
    
}

public partial class AbilitySystemViewer : _AbilitySystemInstanceWrapper_<VBoxContainer>
{
    public AbilitySystemViewer(VBoxContainer _instance) : base (_instance) {}
    
    public AbilitySystemViewer() : this((VBoxContainer)ClassDB.Instantiate("AbilitySystemViewer")) { }
    
    public static new class MethodName
    {
    }
    
    public static new class PropertyName
    {
        public static readonly StringName AbilitySystemPath = "ability_system_path";
    }
    
    public NodePath AbilitySystemPath
    {
        get => (NodePath)ClassDB.ClassGetProperty(Instance, PropertyName.AbilitySystemPath);
        set => ClassDB.ClassSetProperty(Instance, PropertyName.AbilitySystemPath, value);
    }
    
}

public partial class AttributeViewer : _AbilitySystemInstanceWrapper_<Control>
{
    public AttributeViewer(Control _instance) : base (_instance) {}
    
    public AttributeViewer() : this((Control)ClassDB.Instantiate("AttributeViewer")) { }
    
    public static new class MethodName
    {
    }
    
    public static new class PropertyName
    {
    }
    
}

public partial class AbilityViewer : _AbilitySystemInstanceWrapper_<Control>
{
    public AbilityViewer(Control _instance) : base (_instance) {}
    
    public AbilityViewer() : this((Control)ClassDB.Instantiate("AbilityViewer")) { }
    
    public static new class MethodName
    {
    }
    
    public static new class PropertyName
    {
    }
    
}

public partial class EventViewer : _AbilitySystemInstanceWrapper_<Control>
{
    public EventViewer(Control _instance) : base (_instance) {}
    
    public EventViewer() : this((Control)ClassDB.Instantiate("EventViewer")) { }
    
    public static new class MethodName
    {
    }
    
    public static new class PropertyName
    {
    }
    
}

public partial class TagViewer : _AbilitySystemInstanceWrapper_<Control>
{
    public TagViewer(Control _instance) : base (_instance) {}
    
    public TagViewer() : this((Control)ClassDB.Instantiate("TagViewer")) { }
    
    public static new class MethodName
    {
    }
    
    public static new class PropertyName
    {
    }
    
}

public interface _IAbilitySystemInstanceWrapper_ : IDisposable
{
    void ClearNativePointer();
}

public partial class _AbilitySystemInstanceWrapper_<T> : _IAbilitySystemInstanceWrapper_ where T: GodotObject
{
    public static class Enum { }
    public static class PropertyName { }
    public static class MethodName { }
    public static class SignalName { }
    public T Instance { get; protected set; }
    
    public _AbilitySystemInstanceWrapper_(T _instance)
    {
        if (_instance == null) throw new ArgumentNullException(nameof(_instance));
        if (!ClassDB.IsParentClass(_instance.GetClass(), GetType().Name)) throw new ArgumentException("\"_instance\" has the wrong type.");
        Instance = _instance;
    }
    
    public void Dispose()
    {
        Instance?.Dispose();
        Instance = null;
    }
    
    public void ClearNativePointer() => Instance = null;
}

internal static class _AbilitySystemUtils_
{
    
    static System.Collections.Generic.Dictionary<ulong, _IAbilitySystemInstanceWrapper_> cached_instances = new();
    static DateTime previous_clear_time = DateTime.Now;
    
    public static object CreateWrapperFromObject(GodotObject _instance)
    {
        if (_instance == null)
        {
            return null;
        }
        
        ulong id = _instance.GetInstanceId();
        if (cached_instances.ContainsKey(id))
        {
            return cached_instances[id];
        }
        
        if ((DateTime.Now - previous_clear_time).TotalSeconds > 1)
        {
            var query = cached_instances.Where((i) => GodotObject.IsInstanceIdValid(i.Key)).ToArray();
            foreach (var i in query)
            {
                i.Value.ClearNativePointer();
                cached_instances.Remove(i.Key);
            }
            previous_clear_time = DateTime.Now;
        }
        
        switch(_instance.GetClass())
        {
            case "Ability":
            {
                _IAbilitySystemInstanceWrapper_ new_instance = new Ability((Resource)_instance);
                cached_instances[id] = new_instance;
                return new_instance;
            }
            case "AbilityEvent":
            {
                _IAbilitySystemInstanceWrapper_ new_instance = new AbilityEvent((Resource)_instance);
                cached_instances[id] = new_instance;
                return new_instance;
            }
            case "AbilitySystem":
            {
                _IAbilitySystemInstanceWrapper_ new_instance = new AbilitySystem((Node)_instance);
                cached_instances[id] = new_instance;
                return new_instance;
            }
            case "Attribute":
            {
                _IAbilitySystemInstanceWrapper_ new_instance = new Attribute((Resource)_instance);
                cached_instances[id] = new_instance;
                return new_instance;
            }
            case "Tag":
            {
                _IAbilitySystemInstanceWrapper_ new_instance = new Tag((Resource)_instance);
                cached_instances[id] = new_instance;
                return new_instance;
            }
            case "Effect":
            {
                _IAbilitySystemInstanceWrapper_ new_instance = new Effect((Resource)_instance);
                cached_instances[id] = new_instance;
                return new_instance;
            }
            case "AttributeEffect":
            {
                _IAbilitySystemInstanceWrapper_ new_instance = new AttributeEffect((Resource)_instance);
                cached_instances[id] = new_instance;
                return new_instance;
            }
            case "LoopEffect":
            {
                _IAbilitySystemInstanceWrapper_ new_instance = new LoopEffect((Resource)_instance);
                cached_instances[id] = new_instance;
                return new_instance;
            }
            case "TagEffect":
            {
                _IAbilitySystemInstanceWrapper_ new_instance = new TagEffect((Resource)_instance);
                cached_instances[id] = new_instance;
                return new_instance;
            }
            case "TryActivateAbilityEffect":
            {
                _IAbilitySystemInstanceWrapper_ new_instance = new TryActivateAbilityEffect((Resource)_instance);
                cached_instances[id] = new_instance;
                return new_instance;
            }
            case "WaitEffect":
            {
                _IAbilitySystemInstanceWrapper_ new_instance = new WaitEffect((Resource)_instance);
                cached_instances[id] = new_instance;
                return new_instance;
            }
            case "AbilitySystemViewer":
            {
                _IAbilitySystemInstanceWrapper_ new_instance = new AbilitySystemViewer((VBoxContainer)_instance);
                cached_instances[id] = new_instance;
                return new_instance;
            }
            case "AttributeViewer":
            {
                _IAbilitySystemInstanceWrapper_ new_instance = new AttributeViewer((Control)_instance);
                cached_instances[id] = new_instance;
                return new_instance;
            }
            case "AbilityViewer":
            {
                _IAbilitySystemInstanceWrapper_ new_instance = new AbilityViewer((Control)_instance);
                cached_instances[id] = new_instance;
                return new_instance;
            }
            case "EventViewer":
            {
                _IAbilitySystemInstanceWrapper_ new_instance = new EventViewer((Control)_instance);
                cached_instances[id] = new_instance;
                return new_instance;
            }
            case "TagViewer":
            {
                _IAbilitySystemInstanceWrapper_ new_instance = new TagViewer((Control)_instance);
                cached_instances[id] = new_instance;
                return new_instance;
            }
        }
        throw new NotImplementedException();
    }
}
