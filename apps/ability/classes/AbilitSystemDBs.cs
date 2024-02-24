using Godot;
using AS;

public class AbilityDB : SingletonDB<Ability>
{
    public static Ability? Find(StringName identifier) => Instance.Resources.Find(ability => ability.Identifier == identifier);
}

public class AttributeDB : SingletonDB<Attribute>
{
    public static Attribute? Find(StringName identifier) => Instance.Resources.Find(attribute => attribute.Identifier == identifier);
}

public class EffectDB : SingletonDB<Effect>
{
    public static Effect? FindByName(StringName name) => Instance.Resources.Find(effect => effect.UIName == name);
}

public class TagDB : SingletonDB<Tag>
{
    public static Tag? Find(StringName identifier) => Instance.Resources.Find(tag => tag.Identifier == identifier);
}