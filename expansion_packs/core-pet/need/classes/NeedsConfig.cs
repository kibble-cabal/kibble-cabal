using System.Linq;
using Godot;

public class NeedsConfig : Singleton<NeedsConfig>
{
    public StringName[] Needs = [
        "hunger",
        "thirst",
        "energy",
        "activity"
    ];

    public StringName[] FulfillNeeds = [
        "eat",
        "sleep",
        "drink",
        "play"
    ];

    public Attribute[] NeedAttributes => [.. Needs.Select(AttributeDB.Find).WhereNotNull()];
    public Ability[] FulfillNeedsAbilities => [.. FulfillNeeds.Select(AbilityDB.Find).WhereNotNull()];
}