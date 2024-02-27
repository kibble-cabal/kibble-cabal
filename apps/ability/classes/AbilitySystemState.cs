using System.Collections.Generic;
using System.Linq;
using Godot;
using AS;
using GDC = Godot.Collections;

/// <summary>
///  This class uses the AbilityDB, TagDB, and AttributeDB to populate the state of an AbilitySystem node.
/// This class stores the state of an AbilitySystem node by storing just the IDENTIFIERS of the node's Attributes, Abilities, and Tags.
/// It's done this way to ensure that there are no outdated instances of abilities, tags, etc. serialized anywhere.
/// </summary>
[Tool]
[GlobalClass]
public partial class AbilitySystemState : Resource
{
    public List<AbilityEvent> Events = [];

    [Export]
    public GDC.Array<StringName> Abilities = [];

    [Export]
    public GDC.Dictionary<StringName, float> Attributes = [];

    [Export]
    public GDC.Array<StringName> Tags = [];

    [Export(PropertyHint.TypeString, "28:24/17:AbilityEvent")] // TODO this isn't working
    private GDC.Array<Resource> EventResources
    {
        get => new(Events.Convert<AbilityEvent, Resource>());
        set => Events = value.Convert<Resource, AbilityEvent>().ToList();
    }

    public IEnumerable<Tag> GetTags() => Tags.Select(TagDB.Find).WhereNotNull();
    public IEnumerable<Ability> GetAbilities() => Abilities.Select(AbilityDB.Find).WhereNotNull();
    public Dictionary<Attribute, float> GetAttributes() => Attributes.Keys
            .Select(attr => (Attribute: AttributeDB.Find(attr), Value: Attributes[attr]))
            .Where(attr => attr.Attribute is not null)
            .Select(attr => (attr.Attribute!, attr.Value))
            .ToDictionary();

    /// <summary>
    /// Creates a new resource by populating tags, attributes, and abilities FROM the provided node.
    /// </summary>
    public static AbilitySystemState From(AbilitySystem system) => new()
    {
        Attributes = system.Attributes.Keys
                .Select(key => key.TryAs<GodotObject>() as Resource)
                .WhereNotNull()
                .Select(obj => new Attribute(obj))
                .Select(attr => (attr.Identifier, system.GetAttributeValue(attr)))
                .ToGodotDictionary(),
        Tags = system.Tags
                .Select(tag => tag.Identifier)
                .ToGodotArray(),
        Abilities = system.Abilities
                .Select(ability => ability.Identifier)
                .ToGodotArray(),
        Events = [.. system.Events]
    };

    /// <summary>
    /// Modifies the provided AbilitySystem node by replacing its tags, attributes, and abilities from this resource.
    /// </summary>
    public void To(AbilitySystem system)
    {
        system.Attributes.Clear();
        GetAttributes().ForEach((attr, val) =>
        {
            system.GrantAttribute(attr);
            system.SetAttributeValue(attr, val);
        });
        system.Tags.Clear();
        GetTags().ForEach(system.GrantTag);
        GetAbilities().ForEach(system.GrantAbility);
        system.Events = Events;
    }

    /// <summary>
    /// Modifies this resource by merging tags, attributes, and abilities from the provided state into this resource.
    /// </summary>
    public void MergeWith(AbilitySystemState other)
    {
        Attributes.Merge(other.Attributes);
        Tags.AddRange(other.Tags.Except(Tags));
        Abilities.AddRange(other.Abilities.Except(Abilities));
        Events.AddRange(other.Events.Except(Events));
    }

    /// <summary>
    /// Modifies this resource by merging tags, attributes, and abilities from the provided AbilitySystem into this resource.
    /// </summary>
    public void MergeWith(AbilitySystem system) => MergeWith(From(system));

    /// <summary>
    /// Modifies the provided node  by merging tags, attributes, and abilities from this resource into it.
    /// </summary>
    public void MergeInto(AbilitySystem system)
    {
        var state = (Duplicate() as AbilitySystemState)!;
        state.MergeWith(system);
        state.To(system);
    }

    public void AddAttributes(IEnumerable<Attribute> attributes)
    {
        foreach (var attr in attributes)
        {
            if (!Attributes.ContainsKey(attr.Identifier))
                Attributes[attr.Identifier] = attr.DefaultValue;
        }
    }

    static AbilitySystemState()
    {
        #if TOOLS
        const string basePath = "res://docs/schemas/ability_system";
        registerGenerator<Ability>([Ability.Property.Identifier]);
        registerGenerator<Attribute>([Attribute.Property.Identifier]);
        registerGenerator<Tag>([Tag.Property.Identifier]);
        registerGenerator<Effect>(exclude: [Effect.Property.ElapsedTime]);
        registerGenerator<Attribute>();
        registerGenerator<LoopEffect>();
        registerGenerator<WaitEffect>();
        registerGenerator<TryActivateAbilityEffect>();
        registerGenerator<AbilityEvent>();
        JSONSchema.GeneratorDB.Register(new JSONSchema.Generator
        {
            ClassName = nameof(AbilitySystemState),
            Path = "res://docs/schemas/ability_system/AbilitySystemState.schema.json",
            Title = "Ability System State"
        });
        return;

        void registerGenerator<Ty>(StringName[]? require = null, StringName[]? exclude = null) => JSONSchema.GeneratorDB.Register(new JSONSchema.Generator
        {
            ClassName = typeof(Ty).Name,
            Path = $"{basePath}/{typeof(Ty).Name}.schema.json",
            Title = typeof(Ty).Name,
            RequiredProperties = require ?? [],
            ExcludeProperties = exclude ?? [],
            ConvertCase = true,
        });
        #endif
    }
}