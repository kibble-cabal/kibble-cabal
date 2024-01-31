# core-pet/main.gd

extends Object

const Subtrees = [
	preload("ai/resources/fulfill_lowest_need_subtree.tres")
]

const Abilities = [
	preload("need/resources/abilities/eat.ability.tres"),
	preload("need/resources/abilities/eat_cooldown.ability.tres"),
	preload("need/resources/abilities/drink.ability.tres"),
	preload("need/resources/abilities/drink_cooldown.ability.tres"),
	preload("need/resources/abilities/sleep.ability.tres"),
	preload("need/resources/abilities/sleep_cooldown.ability.tres"),
	preload("need/resources/abilities/play.ability.tres"),
]

const NeedAttributes = [
	preload("need/resources/attributes/activity.attribute.tres"),
	preload("need/resources/attributes/hunger.attribute.tres"),
	preload("need/resources/attributes/thirst.attribute.tres"),
	preload("need/resources/attributes/energy.attribute.tres"),
]

const PersonalityAttributes = [
	preload("personality/resources/attributes/agreeableness.tres"),
	preload("personality/resources/attributes/conscientiousness.tres"),
	preload("personality/resources/attributes/extraversion.tres"),
	preload("personality/resources/attributes/neuroticism.tres"),
	preload("personality/resources/attributes/openness.tres"),
]

const Animals = [
	preload("animal/resources/dog.tres")
]

const Actions = [
	preload("action/resources/fulfill_hunger.instruction.tres"),
	preload("action/resources/fulfill_thirst.instruction.tres"),
	preload("action/resources/fulfill_energy.instruction.tres"),
	preload("action/resources/fulfill_activity.instruction.tres"),
]

const ActionScripts = [
	preload("action/classes/rename_action.gd"),
]

const Tags = [
	preload("need/resources/tags/activity_provider.tag.tres"),
	preload("need/resources/tags/energy_provider.tag.tres"),
	preload("need/resources/tags/hunger_provider.tag.tres"),
	preload("need/resources/tags/thirst_provider.tag.tres"),
	preload("need/resources/tags/just_ate.tag.tres"),
	preload("need/resources/tags/just_drank.tag.tres"),
	preload("need/resources/tags/just_slept.tag.tres"),
]


func _init() -> void:
	Subtrees.map(SubtreeDB.register)
	Abilities.map(AbilityDB.register)
	NeedAttributes.map(AttributeDB.register)
	PersonalityAttributes.map(AttributeDB.register)
	Animals.map(AnimalDB.register)
	Tags.map(TagDB.register)
	Actions.map(ActionDB.register)
	ActionScripts.map(func(action: GDScript) -> void: ActionDB.register(action.new()))
	
	await Meta.systems_ready
	LuaSystem.expose_lua_objects.append(NeedsLuaAPI.new())
	DatetimeSystem.ticked.connect(deplete_needs)


func deplete_needs() -> void:
	for need_identifier in NeedsConfig.Needs:
		var need := AttributeDB.find(need_identifier) as NeedAttribute
		if not need: continue
		for pet_node in PetSystem.get_pet_nodes():
			var modifier := randf_range(0.01, 0.02) * need.depletion_rate
			pet_node.ability_system.modify_attribute_value(need, -modifier)
			
