using System.Collections.Generic;
using System.Linq;
using Godot;
using AS;

public class PersonalityConfig : Singleton<PersonalityConfig>
{
    public StringName[] Characteristics = [
        "conscientiousness",
        "extraversion",
        "neuroticism",
        "agreeableness",
        "openness",
    ];

    public IEnumerable<Attribute> CharacteristicAttributes => Characteristics.Select(AttributeDB.Find).WhereNotNull();

    public void RandomizePersonality(AbilitySystem system, bool overwrite = false) => CharacteristicAttributes.ForEach(attribute =>
        {
            var hasAttribute = system.HasAttribute(attribute);
            if (!hasAttribute) system.GrantAttribute(attribute);
            if (!hasAttribute || overwrite) system.SetAttributeValue(attribute, attribute.RandomValue());
        });
}