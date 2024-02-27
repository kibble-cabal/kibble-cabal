
using System.Collections.Generic;
using Godot;
using AS;

namespace KibbleCabal.Core.Pet
{
    public partial class Main : GodotObject
    {
        public static readonly string BasePath = "res://expansion_packs/core-pet";

        public static readonly IEnumerable<IContextAction> Actions = [
            GD.Load<IContextAction>($"{BasePath}/action/resources/FulfillActivity.instruction.tres"),
            GD.Load<IContextAction>($"{BasePath}/action/resources/FulfillEnergy.instruction.tres"),
            GD.Load<IContextAction>($"{BasePath}/action/resources/FulfillHunger.instruction.tres"),
            GD.Load<IContextAction>($"{BasePath}/action/resources/FulfillThirst.instruction.tres"),
            new RenameContextAction()
        ];

        public static readonly IEnumerable<RAnimal> Animals = [
            GD.Load<RAnimal>($"{BasePath}/animal/resources/Dog.tres"),
        ];

        public static readonly IEnumerable<Attribute> Attributes = [
            GD.Load<Resource>($"{BasePath}/need/resources/attributes/activity.attribute.tres"),
            GD.Load<Resource>($"{BasePath}/need/resources/attributes/energy.attribute.tres"),
            GD.Load<Resource>($"{BasePath}/need/resources/attributes/hunger.attribute.tres"),
            GD.Load<Resource>($"{BasePath}/need/resources/attributes/thirst.attribute.tres"),
            GD.Load<Resource>($"{BasePath}/personality/resources/attributes/agreeableness.tres"),
            GD.Load<Resource>($"{BasePath}/personality/resources/attributes/conscientiousness.tres"),
            GD.Load<Resource>($"{BasePath}/personality/resources/attributes/extraversion.tres"),
            GD.Load<Resource>($"{BasePath}/personality/resources/attributes/neuroticism.tres"),
            GD.Load<Resource>($"{BasePath}/personality/resources/attributes/openness.tres"),
        ];

        public static readonly IEnumerable<Ability> Abilities = [
            GD.Load<Resource>($"{BasePath}/need/resources/abilities/drink.ability.tres"),
            GD.Load<Resource>($"{BasePath}/need/resources/abilities/drink_cooldown.ability.tres"),
            GD.Load<Resource>($"{BasePath}/need/resources/abilities/eat.ability.tres"),
            GD.Load<Resource>($"{BasePath}/need/resources/abilities/eat_cooldown.ability.tres"),
            GD.Load<Resource>($"{BasePath}/need/resources/abilities/play.ability.tres"),
            GD.Load<Resource>($"{BasePath}/need/resources/abilities/sleep.ability.tres"),
            GD.Load<Resource>($"{BasePath}/need/resources/abilities/sleep_cooldown.ability.tres"),
        ];

        public static readonly IEnumerable<Tag> Tags = [
            GD.Load<Resource>($"{BasePath}/need/resources/tags/activity_provider.tag.tres"),
            GD.Load<Resource>($"{BasePath}/need/resources/tags/energy_provider.tag.tres"),
            GD.Load<Resource>($"{BasePath}/need/resources/tags/hunger_provider.tag.tres"),
            GD.Load<Resource>($"{BasePath}/need/resources/tags/thirst_provider.tag.tres"),
            GD.Load<Resource>($"{BasePath}/need/resources/tags/just_ate.tag.tres"),
            GD.Load<Resource>($"{BasePath}/need/resources/tags/just_drank.tag.tres"),
            GD.Load<Resource>($"{BasePath}/need/resources/tags/just_slept.tag.tres"),
        ];

        public Main()
        {
            Actions.ForEach(ContextActionDB.Register);
            Animals.ForEach(AnimalDB.Register);
            Attributes.ForEach(AttributeDB.Register);
            Abilities.ForEach(AbilityDB.Register);
            Tags.ForEach(TagDB.Register);

            DateTimeSubSystem.Instance.Ticked += DepleteNeeds;
        }

        public static void DepleteNeeds()
        {
            foreach (var pet in LocationSubSystem.GetPetSpawners())
                NeedsConfig.Instance.NeedAttributes.ForEach(attr =>
                {
                    float modifier = (float)GD.RandRange(0.01f, 0.02f) * attr.Instance?.Get("depletion_rate").As<float>() ?? default;
                    pet.Node?.AbilitySystem?.ModifyAttributeValue(attr, -modifier);
                });
        }
    }
}