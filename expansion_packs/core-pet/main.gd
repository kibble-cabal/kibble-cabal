# core-pet/main.gd

extends Object

const Subtrees = [
	preload("res://expansion_packs/core-pet/ai/resources/fulfill_lowest_need_subtree.tres")
]

const Abilities = [
	preload("res://expansion_packs/core-pet/ability/resources/abilities/eat.ability.tres"),
	preload("res://expansion_packs/core-pet/ability/resources/abilities/eat_cooldown.ability.tres"),
	preload("res://expansion_packs/core-pet/ability/resources/abilities/drink.ability.tres"),
	preload("res://expansion_packs/core-pet/ability/resources/abilities/drink_cooldown.ability.tres"),
	preload("res://expansion_packs/core-pet/ability/resources/abilities/sleep.ability.tres"),
	preload("res://expansion_packs/core-pet/ability/resources/abilities/sleep_cooldown.ability.tres"),
	preload("res://expansion_packs/core-pet/ability/resources/abilities/play.ability.tres"),
]

const Attributes = [
	preload("res://expansion_packs/core-pet/ability/resources/attributes/activity.attribute.tres"),
	preload("res://expansion_packs/core-pet/ability/resources/attributes/hunger.attribute.tres"),
	preload("res://expansion_packs/core-pet/ability/resources/attributes/thirst.attribute.tres"),
	preload("res://expansion_packs/core-pet/ability/resources/attributes/energy.attribute.tres"),
]

const Animals = [
	preload("res://expansion_packs/core-pet/animal/resources/dog.tres")
]

const Actions = [
	preload("res://expansion_packs/core-pet/action/resources/fulfill_hunger.instruction.tres"),
	preload("res://expansion_packs/core-pet/action/resources/fulfill_thirst.instruction.tres"),
	preload("res://expansion_packs/core-pet/action/resources/fulfill_energy.instruction.tres"),
	preload("res://expansion_packs/core-pet/action/resources/fulfill_activity.instruction.tres"),
]

const ActionScripts = [
	preload("res://expansion_packs/core-pet/action/classes/rename_action.gd"),
]

const Tags = [
	preload("res://expansion_packs/core-pet/ability/resources/tags/activity_provider.tag.tres"),
	preload("res://expansion_packs/core-pet/ability/resources/tags/energy_provider.tag.tres"),
	preload("res://expansion_packs/core-pet/ability/resources/tags/hunger_provider.tag.tres"),
	preload("res://expansion_packs/core-pet/ability/resources/tags/thirst_provider.tag.tres"),
	preload("res://expansion_packs/core-pet/ability/resources/tags/just_ate.tag.tres"),
	preload("res://expansion_packs/core-pet/ability/resources/tags/just_drank.tag.tres"),
	preload("res://expansion_packs/core-pet/ability/resources/tags/just_slept.tag.tres"),
]


func _init() -> void:
	Subtrees.map(SubtreeDB.register)
	Abilities.map(AbilityDB.register)
	Attributes.map(AttributeDB.register)
	Animals.map(AnimalDB.register)
	Tags.map(TagDB.register)
	Actions.map(ActionDB.register)
	ActionScripts.map(func(action: GDScript) -> void: ActionDB.register(action.new()))
	
	await Meta.systems_ready
	DatetimeSystem.ticked.connect(deplete_needs)


func deplete_needs() -> void:
	for need_identifier in NeedsConfig.Needs:
		var need := AttributeDB.find(need_identifier) as NeedAttribute
		if not need: continue
		for pet_node in PetSystem.pet_nodes:
			var modifier := randf_range(0.01, 0.02) * need.depletion_rate
			(pet_node as PetScene).ability_system.modify_attribute_value(need, -modifier)
			
