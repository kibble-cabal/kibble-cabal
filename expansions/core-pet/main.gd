# core-pet/main.gd

extends Object

const Subtrees = [
	preload("res://expansions/core-pet/content/resources/behavior_trees/fulfill_lowest_need_subtree.tres")
]

const Abilities = [
	preload("res://expansions/core-pet/content/resources/abilities/eat.ability.tres"),
	preload("res://expansions/core-pet/content/resources/abilities/eat_cooldown.ability.tres"),
]

const Attributes = [
	preload("res://expansions/core-pet/content/resources/attributes/activity.attribute.tres"),
	preload("res://expansions/core-pet/content/resources/attributes/hunger.attribute.tres"),
	preload("res://expansions/core-pet/content/resources/attributes/thirst.attribute.tres"),
	preload("res://expansions/core-pet/content/resources/attributes/energy.attribute.tres"),
]

const Animals = [
	preload("res://expansions/core-pet/animals/dog/dog.tres")
]

const Actions = [
	preload("res://expansions/core-pet/features/actions/rename_action.gd")
]


func _init() -> void:
	Subtrees.map(SubtreeDB.register)
	Abilities.map(AbilityDB.register)
	Attributes.map(AttributeDB.register)
	Animals.map(AnimalDB.register)
	Actions.map(func(action: GDScript) -> void: ActionDB.register(action.new()))
