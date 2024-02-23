using System.Collections.Generic;
using System.Linq;
using Godot;

using GDC = Godot.Collections;

/// <summary>
///  This class uses the AbilityDB, TagDB, and AttributeDB to populate the state of an AbilitySystem node.
/// This class stores the state of an AbilitySystem node by storing just the IDENTIFIERS of the node's Attributes, Abilitys, and Tags.
/// It's done this way to ensure that there are no outdated instances of abilities, tags, etc. serialized anywhere.
/// </summary>
public partial class AbilitySystemState : Resource
{
    [Export]
    public GDC.Array<StringName> Abilities = [];

    [Export]
    public GDC.Dictionary<StringName, float> Attributes = [];

    [Export]
    public GDC.Array<StringName> Tags = [];

    [Export]
    private GDC.Array<Resource> EventResources
    {
        get => Events.Select(e => e.Instance).ToGodotArray();
        set => Events = value.Select(e => new AbilityEvent(e)).ToList();
    }
    public List<AbilityEvent> Events = [];

    /// <summary>
    /// Creates a new resource by populating tags, attributes, and abilities FROM the provided node.
    /// </summary>
    public static AbilitySystemState From(AbilitySystem system) => new()
    {
        Attributes = system.Attributes.Keys
                .ConvertTo<Resource, Attribute>()
                .Select(attr => (attr.Identifier, system.GetAttributeValue(attr)))
                .ToGodotDictionary(),
        Tags = system.Tags
                .ConvertTo<Resource, Tag>()
                .Select(tag => tag.Identifier)
                .ToGodotArray(),
        Abilities = system.Abilities
                .ConvertTo<Resource, Ability>()
                .Select(ability => ability.Identifier)
                .ToGodotArray(),
        Events = system.Events
                .ConvertTo<Resource, AbilityEvent>()
                .ToList()
    };

    /// <summary>
    /// Modifies the provided AbilitySystem node by replacing its tags, attributes, and abilities from this resource.
    /// </summary>
    public void To(AbilitySystem system)
    {
        system.Attributes.Clear();
        Attributes.Keys.Select(AttributeDB.Find).WhereNotNull().ForEach(attr =>
        {
            system.GrantAttribute(attr);
            system.SetAttributeValue(attr, Attributes[attr.Identifier]);
        });
        system.Tags.Clear();
        Tags.Select(TagDB.Find).WhereNotNull().ForEach(system.GrantTag);
        Abilities.Select(AbilityDB.Find).WhereNotNull().ForEach(system.GrantAbility);
        system.Events = (GDC.Array)EventResources;
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
}