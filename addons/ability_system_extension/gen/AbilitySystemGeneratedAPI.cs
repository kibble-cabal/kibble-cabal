/// 0.1.0-alpha
/// ////////////////////////////////////////////////
/// THIS FILE HAS BEEN GENERATED.
/// THE CHANGES IN THIS FILE WILL BE OVERWRITTEN
/// AFTER THE UPDATE OR AFTER THE RESTART!
/// ////////////////////////////////////////////////

using Godot;
using System.Linq;
using System.Collections.Generic;

#nullable enable

namespace AS
{
    public static class ClassNames
    {
        public static readonly StringName Ability = "Ability";
        public static readonly StringName AbilityEvent = "AbilityEvent";
        public static readonly StringName AbilitySystem = "AbilitySystem";
        public static readonly StringName Attribute = "Attribute";
        public static readonly StringName Tag = "Tag";
        public static readonly StringName Effect = "Effect";
        public static readonly StringName AttributeEffect = "AttributeEffect";
        public static readonly StringName LoopEffect = "LoopEffect";
        public static readonly StringName TagEffect = "TagEffect";
        public static readonly StringName TryActivateAbilityEffect = "TryActivateAbilityEffect";
        public static readonly StringName WaitEffect = "WaitEffect";
        public static readonly StringName AbilitySystemViewer = "AbilitySystemViewer";
        public static readonly StringName AttributeViewer = "AttributeViewer";
        public static readonly StringName AbilityViewer = "AbilityViewer";
        public static readonly StringName EventViewer = "EventViewer";
        public static readonly StringName TagViewer = "TagViewer";
    }
    
    [Tool]
    public partial class Ability : IInstanceWrapper<Resource>
    {
        [Export]
        public Resource Instance { get; set; }

        public Ability(Resource instance) => (this as IInstanceWrapper<Resource>).SetInstance(instance);

        public static implicit operator Ability(Resource? instance) => instance is null ? new() : new(instance);
        public static implicit operator Resource(Ability obj) => obj.Instance;

        public static implicit operator Variant(Ability? obj) => obj is null ? new() : obj.Instance;

        public Ability() : this((Resource)ClassDB.Instantiate(ClassNames.Ability)) { }

        public static class Enum
        {
            public enum Mode : long
            {
                Parallel = 1,
                Sequential = 2,
            }
        }

        public static class Method
        {
        }

        public static class Property
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
            get => ClassDB.ClassGetProperty(Instance, Property.Identifier).As<StringName>();
            set => ClassDB.ClassSetProperty(Instance, Property.Identifier, value);
        }

        public List<Tag> TagsBlocking
        {
            get => ClassDB.ClassGetProperty(Instance, Property.TagsBlocking).Convert<Resource, Tag>().ToList();
            set => ClassDB.ClassSetProperty(Instance, Property.TagsBlocking, new Godot.Collections.Array<Resource>(value.Convert<Tag, Resource>()));
        }

        public List<Tag> TagsRequired
        {
            get => ClassDB.ClassGetProperty(Instance, Property.TagsRequired).Convert<Resource, Tag>().ToList();
            set => ClassDB.ClassSetProperty(Instance, Property.TagsRequired, new Godot.Collections.Array<Resource>(value.Convert<Tag, Resource>()));
        }

        public List<Effect> Effects
        {
            get => ClassDB.ClassGetProperty(Instance, Property.Effects).Convert<Resource, Effect>().ToList();
            set => ClassDB.ClassSetProperty(Instance, Property.Effects, new Godot.Collections.Array<Resource>(value.Convert<Effect, Resource>()));
        }

        public int EffectMode
        {
            get => ClassDB.ClassGetProperty(Instance, Property.EffectMode).As<int>();
            set => ClassDB.ClassSetProperty(Instance, Property.EffectMode, value);
        }

        public Color UIColor
        {
            get => ClassDB.ClassGetProperty(Instance, Property.UIColor).As<Color>();
            set => ClassDB.ClassSetProperty(Instance, Property.UIColor, value);
        }

        public override string ToString() => Instance.ToString();
    }

    [Tool]
    public partial class AbilityEvent : IInstanceWrapper<Resource>
    {
        [Export]
        public Resource Instance { get; set; }

        public AbilityEvent(Resource instance) => (this as IInstanceWrapper<Resource>).SetInstance(instance);

        public static implicit operator AbilityEvent(Resource? instance) => instance is null ? new() : new(instance);
        public static implicit operator Resource(AbilityEvent obj) => obj.Instance;

        public static implicit operator Variant(AbilityEvent? obj) => obj is null ? new() : obj.Instance;

        public AbilityEvent() : this((Resource)ClassDB.Instantiate(ClassNames.AbilityEvent)) { }

        public static class Method
        {
        }

        public static class Property
        {
            public static readonly StringName Ability = "ability";
            public static readonly StringName EffectInstances = "effect_instances";
        }

        public Ability? Ability
        {
            get => ClassDB.ClassGetProperty(Instance, Property.Ability).As<Resource?>();
            set => ClassDB.ClassSetProperty(Instance, Property.Ability, Variant.From(value));
        }

        public List<Effect> EffectInstances
        {
            get => ClassDB.ClassGetProperty(Instance, Property.EffectInstances).Convert<Resource, Effect>().ToList();
            set => ClassDB.ClassSetProperty(Instance, Property.EffectInstances, new Godot.Collections.Array<Resource>(value.Convert<Effect, Resource>()));
        }

        public override string ToString() => Instance.ToString();
    }

    [Tool]
    public partial class AbilitySystem : IInstanceWrapper<Node>
    {
        [Export]
        public Node Instance { get; set; }

        public AbilitySystem(Node instance) => (this as IInstanceWrapper<Node>).SetInstance(instance);

        public static implicit operator AbilitySystem(Node? instance) => instance is null ? new() : new(instance);
        public static implicit operator Node(AbilitySystem obj) => obj.Instance;

        public static implicit operator Variant(AbilitySystem? obj) => obj is null ? new() : obj.Instance;

        public AbilitySystem() : this((Node)ClassDB.Instantiate(ClassNames.AbilitySystem)) { }

        public static class Enum
        {
            public enum UpdateMode : long
            {
                Disabled = 0,
                Physics = 1,
                Process = 2,
            }
        }

        public static class Method
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


        public bool HasAttribute(Attribute? attribute) => Instance?.Call(Method.HasAttribute, attribute).As<bool>() ?? default;

        public void GrantAttribute(Attribute? attribute) => Instance?.Call(Method.GrantAttribute, attribute);

        public void RevokeAttribute(Attribute? attribute) => Instance?.Call(Method.RevokeAttribute, attribute);

        public float GetAttributeValue(Attribute? attribute) => Instance?.Call(Method.GetAttributeValue, attribute).As<float>() ?? default;

        public void SetAttributeValue(Attribute? attribute, float value) => Instance?.Call(Method.SetAttributeValue, attribute, value);

        public void ModifyAttributeValue(Attribute? attribute, float by_amount) => Instance?.Call(Method.ModifyAttributeValue, attribute, by_amount);

        public bool CanActivate(Ability? ability) => Instance?.Call(Method.CanActivate, ability).As<bool>() ?? default;

        public bool HasAbility(Ability? ability) => Instance?.Call(Method.HasAbility, ability).As<bool>() ?? default;

        public void GrantAbility(Ability? ability) => Instance?.Call(Method.GrantAbility, ability);

        public void RevokeAbility(Ability? ability) => Instance?.Call(Method.RevokeAbility, ability);

        public AbilityEvent? Activate(Ability? ability) => Instance?.Call(Method.Activate, ability).TryAs<Resource>();

        public bool HasTag(Tag? tag) => Instance?.Call(Method.HasTag, tag).As<bool>() ?? default;

        public bool HasSomeTags(List<Tag> tags) => Instance?.Call(Method.HasSomeTags, new Godot.Collections.Array<Resource>(tags.Convert<Tag, Resource>())).As<bool>() ?? default;

        public bool HasAllTags(List<Tag> tags) => Instance?.Call(Method.HasAllTags, new Godot.Collections.Array<Resource>(tags.Convert<Tag, Resource>())).As<bool>() ?? default;

        public void GrantTag(Tag? tag) => Instance?.Call(Method.GrantTag, tag);

        public void RevokeTag(Tag? tag) => Instance?.Call(Method.RevokeTag, tag);

        public static class Property
        {
            public static readonly StringName Tags = "tags";
            public static readonly StringName Abilities = "abilities";
            public static readonly StringName Events = "events";
            public static readonly StringName Attributes = "attributes";
            public static readonly StringName UpdateMode = "update_mode";
        }

        public List<Tag> Tags
        {
            get => ClassDB.ClassGetProperty(Instance, Property.Tags).Convert<Resource, Tag>().ToList();
            set => ClassDB.ClassSetProperty(Instance, Property.Tags, new Godot.Collections.Array<Resource>(value.Convert<Tag, Resource>()));
        }

        public List<Ability> Abilities
        {
            get => ClassDB.ClassGetProperty(Instance, Property.Abilities).Convert<Resource, Ability>().ToList();
            set => ClassDB.ClassSetProperty(Instance, Property.Abilities, new Godot.Collections.Array<Resource>(value.Convert<Ability, Resource>()));
        }

        public List<AbilityEvent> Events
        {
            get => ClassDB.ClassGetProperty(Instance, Property.Events).Convert<Resource, AbilityEvent>().ToList();
            set => ClassDB.ClassSetProperty(Instance, Property.Events, new Godot.Collections.Array<Resource>(value.Convert<AbilityEvent, Resource>()));
        }

        public Godot.Collections.Dictionary Attributes
        {
            get => ClassDB.ClassGetProperty(Instance, Property.Attributes).As<Godot.Collections.Dictionary>();
            set => ClassDB.ClassSetProperty(Instance, Property.Attributes, value);
        }

        public int UpdateMode
        {
            get => ClassDB.ClassGetProperty(Instance, Property.UpdateMode).As<int>();
            set => ClassDB.ClassSetProperty(Instance, Property.UpdateMode, value);
        }

        public override string ToString() => Instance.ToString();
    }

    [Tool]
    public partial class Attribute : IInstanceWrapper<Resource>
    {
        [Export]
        public Resource Instance { get; set; }

        public Attribute(Resource instance) => (this as IInstanceWrapper<Resource>).SetInstance(instance);

        public static implicit operator Attribute(Resource? instance) => instance is null ? new() : new(instance);
        public static implicit operator Resource(Attribute obj) => obj.Instance;
        public static implicit operator Variant(Attribute? obj) => obj is null ? new() : obj.Instance;

        public Attribute() : this((Resource)ClassDB.Instantiate(ClassNames.Attribute)) { }

        public static class Method
        {
        }

        public static class Property
        {
            public static readonly StringName Identifier = "identifier";
            public static readonly StringName MaxValue = "max_value";
            public static readonly StringName MinValue = "min_value";
            public static readonly StringName DefaultValue = "default_value";
            public static readonly StringName UIColor = "ui_color";
        }

        public StringName Identifier
        {
            get => ClassDB.ClassGetProperty(Instance, Property.Identifier).As<StringName>();
            set => ClassDB.ClassSetProperty(Instance, Property.Identifier, value);
        }

        public float MaxValue
        {
            get => ClassDB.ClassGetProperty(Instance, Property.MaxValue).As<float>();
            set => ClassDB.ClassSetProperty(Instance, Property.MaxValue, value);
        }

        public float MinValue
        {
            get => ClassDB.ClassGetProperty(Instance, Property.MinValue).As<float>();
            set => ClassDB.ClassSetProperty(Instance, Property.MinValue, value);
        }

        public float DefaultValue
        {
            get => ClassDB.ClassGetProperty(Instance, Property.DefaultValue).As<float>();
            set => ClassDB.ClassSetProperty(Instance, Property.DefaultValue, value);
        }

        public Color UIColor
        {
            get => ClassDB.ClassGetProperty(Instance, Property.UIColor).As<Color>();
            set => ClassDB.ClassSetProperty(Instance, Property.UIColor, value);
        }

        public override string ToString() => Instance.ToString();
    }

    [Tool]
    public partial class Tag : IInstanceWrapper<Resource>
    {
        [Export]
        public Resource Instance { get; set; }

        public Tag(Resource instance) => (this as IInstanceWrapper<Resource>).SetInstance(instance);

        public static implicit operator Tag(Resource? instance) => instance is null ? new() : new(instance);
        public static implicit operator Resource(Tag obj) => obj.Instance;

        public static implicit operator Variant(Tag? obj) => obj is null ? new() : obj.Instance;

        public Tag() : this((Resource)ClassDB.Instantiate(ClassNames.Tag)) { }

        public static class Method
        {
        }

        public static class Property
        {
            public static readonly StringName Identifier = "identifier";
            public static readonly StringName UIColor = "ui_color";
        }

        public StringName Identifier
        {
            get => ClassDB.ClassGetProperty(Instance, Property.Identifier).As<StringName>();
            set => ClassDB.ClassSetProperty(Instance, Property.Identifier, value);
        }

        public Color UIColor
        {
            get => ClassDB.ClassGetProperty(Instance, Property.UIColor).As<Color>();
            set => ClassDB.ClassSetProperty(Instance, Property.UIColor, value);
        }

        public override string ToString() => Instance.ToString();

    }

    [Tool]
    public partial class Effect : IInstanceWrapper<Resource>
    {
        [Export]
        public Resource Instance { get; set; }

        public Effect(Resource instance) => (this as IInstanceWrapper<Resource>).SetInstance(instance);

        public static implicit operator Effect(Resource? instance) => instance is null ? new() : new(instance);
        public static implicit operator Resource(Effect obj) => obj.Instance;

        public static implicit operator Variant(Effect? obj) => obj is null ? new() : obj.Instance;

        public Effect() : this((Resource)ClassDB.Instantiate(ClassNames.Effect)) { }

        public static class Enum
        {
            public enum Status : long
            {
                Ready = 0,
                Running = 1,
                Finished = 2,
            }
        }

        public static class Method
        {
        }

        public static class Property
        {
            public static readonly StringName ElapsedTime = "elapsed_time";
            public static readonly StringName UIName = "ui_name";
            public static readonly StringName UIColor = "ui_color";
        }

        public float ElapsedTime
        {
            get => ClassDB.ClassGetProperty(Instance, Property.ElapsedTime).As<float>();
            set => ClassDB.ClassSetProperty(Instance, Property.ElapsedTime, value);
        }

        public StringName UIName
        {
            get => ClassDB.ClassGetProperty(Instance, Property.UIName).As<StringName>();
            set => ClassDB.ClassSetProperty(Instance, Property.UIName, value);
        }

        public Color UIColor
        {
            get => ClassDB.ClassGetProperty(Instance, Property.UIColor).As<Color>();
            set => ClassDB.ClassSetProperty(Instance, Property.UIColor, value);
        }
        
        public override string ToString() => Instance.ToString();

    }

    [Tool]
    public partial class AttributeEffect : Effect
    {
        public AttributeEffect(Resource instance) : base(instance) { }

        public static implicit operator AttributeEffect(Resource? instance) => instance is null ? new() : new(instance);
        public static implicit operator Resource(AttributeEffect obj) => obj.Instance;

        public static implicit operator Variant(AttributeEffect? obj) => obj is null ? new() : obj.Instance;

        public AttributeEffect() : this((Resource)ClassDB.Instantiate(ClassNames.AttributeEffect)) { }

        public static new class Method
        {
        }

        public static new class Property
        {
            public static readonly StringName Attribute = "attribute";
            public static readonly StringName MinEffect = "min_effect";
            public static readonly StringName MaxEffect = "max_effect";
        }

        public Attribute? Attribute
        {
            get => ClassDB.ClassGetProperty(Instance, Property.Attribute).As<Resource?>();
            set => ClassDB.ClassSetProperty(Instance, Property.Attribute, Variant.From(value));
        }

        public float MinEffect
        {
            get => ClassDB.ClassGetProperty(Instance, Property.MinEffect).As<float>();
            set => ClassDB.ClassSetProperty(Instance, Property.MinEffect, value);
        }

        public float MaxEffect
        {
            get => ClassDB.ClassGetProperty(Instance, Property.MaxEffect).As<float>();
            set => ClassDB.ClassSetProperty(Instance, Property.MaxEffect, value);
        }

        public override string ToString() => Instance.ToString();
    }

    [Tool]
    public partial class LoopEffect : Effect
    {
        public LoopEffect(Resource instance) : base(instance) { }

        public static implicit operator LoopEffect(Resource? instance) => instance is null ? new() : new(instance);
        public static implicit operator Resource(LoopEffect obj) => obj.Instance;

        public static implicit operator Variant(LoopEffect? obj) => obj is null ? new() : obj.Instance;

        public LoopEffect() : this((Resource)ClassDB.Instantiate(ClassNames.LoopEffect)) { }

        public static new class Method
        {
        }

        public static new class Property
        {
            public static readonly StringName MaxLoops = "max_loops";
            public static readonly StringName ElapsedLoops = "elapsed_loops";
        }

        public int MaxLoops
        {
            get => ClassDB.ClassGetProperty(Instance, Property.MaxLoops).As<int>();
            set => ClassDB.ClassSetProperty(Instance, Property.MaxLoops, value);
        }

        public int ElapsedLoops
        {
            get => ClassDB.ClassGetProperty(Instance, Property.ElapsedLoops).As<int>();
            set => ClassDB.ClassSetProperty(Instance, Property.ElapsedLoops, value);
        }

        public override string ToString() => Instance.ToString();
    }

    [Tool]
    public partial class TagEffect : Effect
    {
        public TagEffect(Resource instance) : base(instance) { }

        public static implicit operator TagEffect(Resource? instance) => instance is null ? new() : new(instance);
        public static implicit operator Resource(TagEffect obj) => obj.Instance;

        public static implicit operator Variant(TagEffect? obj) => obj is null ? new() : obj.Instance;

        public TagEffect() : this((Resource)ClassDB.Instantiate(ClassNames.TagEffect)) { }

        public static new class Enum
        {
            public enum OperationType : long
            {
                Add = 1,
                Remove = -1,
            }
        }

        public static new class Method
        {
        }

        public static new class Property
        {
            public static readonly StringName Tags = "tags";
            public static readonly StringName Operation = "operation";
        }

        public List<Tag> Tags
        {
            get => ClassDB.ClassGetProperty(Instance, Property.Tags).Convert<Resource, Tag>().ToList();
            set => ClassDB.ClassSetProperty(Instance, Property.Tags, new Godot.Collections.Array<Resource>(value.Convert<Tag, Resource>()));
        }

        public int Operation
        {
            get => ClassDB.ClassGetProperty(Instance, Property.Operation).As<int>();
            set => ClassDB.ClassSetProperty(Instance, Property.Operation, value);
        }

        public override string ToString() => Instance.ToString();
    }

    [Tool]
    public partial class TryActivateAbilityEffect : Effect
    {
        public TryActivateAbilityEffect(Resource instance) : base(instance) { }

        public static implicit operator TryActivateAbilityEffect(Resource? instance) => instance is null ? new() : new(instance);
        public static implicit operator Resource(TryActivateAbilityEffect obj) => obj.Instance;

        public static implicit operator Variant(TryActivateAbilityEffect? obj) => obj is null ? new() : obj.Instance;

        public TryActivateAbilityEffect() : this((Resource)ClassDB.Instantiate(ClassNames.TryActivateAbilityEffect)) { }

        public static new class Method
        {
        }

        public static new class Property
        {
            public static readonly StringName Ability = "ability";
        }

        public Ability? Ability
        {
            get => ClassDB.ClassGetProperty(Instance, Property.Ability).As<Resource?>();
            set => ClassDB.ClassSetProperty(Instance, Property.Ability, Variant.From(value));
        }

        public override string ToString() => Instance.ToString();
    }

    public partial class WaitEffect : Effect
    {
        public WaitEffect(Resource instance) : base(instance) { }

        public static implicit operator WaitEffect(Resource? instance) => instance is null ? new() : new(instance);
        public static implicit operator Resource(WaitEffect obj) => obj.Instance;

        public static implicit operator Variant(WaitEffect? obj) => obj is null ? new() : obj.Instance;

        public WaitEffect() : this((Resource)ClassDB.Instantiate(ClassNames.WaitEffect)) { }

        public static new class Method
        {
        }

        public static new class Property
        {
            public static readonly StringName MinWaitTime = "min_wait_time";
            public static readonly StringName MaxWaitTime = "max_wait_time";
            public static readonly StringName WaitTime = "wait_time";
        }

        public float MinWaitTime
        {
            get => ClassDB.ClassGetProperty(Instance, Property.MinWaitTime).As<float>();
            set => ClassDB.ClassSetProperty(Instance, Property.MinWaitTime, value);
        }

        public float MaxWaitTime
        {
            get => ClassDB.ClassGetProperty(Instance, Property.MaxWaitTime).As<float>();
            set => ClassDB.ClassSetProperty(Instance, Property.MaxWaitTime, value);
        }

        public float WaitTime
        {
            get => ClassDB.ClassGetProperty(Instance, Property.WaitTime).As<float>();
            set => ClassDB.ClassSetProperty(Instance, Property.WaitTime, value);
        }

        public override string ToString() => Instance.ToString();
    }

    [Tool]
    public partial class AbilitySystemViewer : IInstanceWrapper<VBoxContainer>
    {
        [Export]
        public VBoxContainer Instance { get; set; }

        public AbilitySystemViewer(VBoxContainer instance) => (this as IInstanceWrapper<VBoxContainer>).SetInstance(instance);

        public static implicit operator AbilitySystemViewer(VBoxContainer? instance) => instance is null ? new() : new(instance);
        public static implicit operator VBoxContainer(AbilitySystemViewer obj) => obj.Instance;

        public static implicit operator Variant(AbilitySystemViewer? obj) => obj is null ? new() : obj.Instance;

        public AbilitySystemViewer() : this((VBoxContainer)ClassDB.Instantiate(ClassNames.AbilitySystemViewer)) { }

        public static class Method
        {
        }

        public static class Property
        {
            public static readonly StringName AbilitySystemPath = "ability_system_path";
        }

        public NodePath AbilitySystemPath
        {
            get => ClassDB.ClassGetProperty(Instance, Property.AbilitySystemPath).As<NodePath>();
            set => ClassDB.ClassSetProperty(Instance, Property.AbilitySystemPath, value);
        }

        public override string ToString() => Instance.ToString();
    }

    [Tool]
    public partial class AttributeViewer : IInstanceWrapper<Control>
    {
        [Export]
        public Control Instance { get; set; }

        public AttributeViewer(Control instance) => (this as IInstanceWrapper<Control>).SetInstance(instance);

        public static implicit operator AttributeViewer(Control? instance) => instance is null ? new() : new(instance);
        public static implicit operator Control(AttributeViewer obj) => obj.Instance;

        public static implicit operator Variant(AttributeViewer? obj) => obj is null ? new() : obj.Instance;

        public AttributeViewer() : this((Control)ClassDB.Instantiate(ClassNames.AttributeViewer)) { }

        public static class Method
        {
        }

        public static class Property
        {
        }

        public override string ToString() => Instance.ToString();
    }

    [Tool]
    public partial class AbilityViewer : IInstanceWrapper<Control>
    {
        [Export]
        public Control Instance { get; set; }

        public AbilityViewer(Control instance) => (this as IInstanceWrapper<Control>).SetInstance(instance);

        public static implicit operator AbilityViewer(Control? instance) => instance is null ? new() : new(instance);
        public static implicit operator Control(AbilityViewer obj) => obj.Instance;

        public static implicit operator Variant(AbilityViewer? obj) => obj is null ? new() : obj.Instance;

        public AbilityViewer() : this((Control)ClassDB.Instantiate(ClassNames.AbilityViewer)) { }

        public static class Method
        {
        }

        public static class Property
        {
        }

        public override string ToString() => Instance.ToString();
    }

    [Tool]
    public partial class EventViewer : IInstanceWrapper<Control>
    {
        [Export]
        public Control Instance { get; set; }

        public EventViewer(Control instance) => (this as IInstanceWrapper<Control>).SetInstance(instance);

        public static implicit operator EventViewer(Control? instance) => instance is null ? new() : new(instance);
        public static implicit operator Control(EventViewer obj) => obj.Instance;

        public static implicit operator Variant(EventViewer? obj) => obj is null ? new() : obj.Instance;

        public EventViewer() : this((Control)ClassDB.Instantiate(ClassNames.EventViewer)) { }

        public static class Method
        {
        }

        public static class Property
        {
        }

    }

    [Tool]
    public partial class TagViewer : IInstanceWrapper<Control>
    {
        [Export]
        public Control Instance { get; set; }

        public TagViewer(Control instance) => (this as IInstanceWrapper<Control>).SetInstance(instance);

        public static implicit operator TagViewer(Control? instance) => instance is null ? new() : new(instance);
        public static implicit operator Control(TagViewer obj) => obj.Instance;

        public static implicit operator Variant(TagViewer? obj) => obj is null ? new() : obj.Instance;

        public TagViewer() : this((Control)ClassDB.Instantiate(ClassNames.TagViewer)) { }

        public static class Method
        {
        }

        public static class Property
        {
        }

        public override string ToString() => Instance.ToString();
    }
}

