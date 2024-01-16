# core-pet/main.gd

extends Object

const Subtrees = [
	preload("res://expansions/core-pet/content/resources/behavior_trees/fulfill_lowest_need_subtree.tres")
]

const Abilities = [
	preload("res://expansions/core-pet/content/resources/abilities/eat.tres")
]

const Attributes = [
	preload("res://expansions/core-pet/content/resources/attributes/activity_attribute.tres"),
	preload("res://expansions/core-pet/content/resources/attributes/hunger_attribute.tres"),
	preload("res://expansions/core-pet/content/resources/attributes/thirst_attribute.tres"),
	preload("res://expansions/core-pet/content/resources/attributes/energy_attribute.tres"),
]


func _init() -> void:
	Subtrees.map(SubtreeDB.register)
	Abilities.map(AbilityDB.register_ability)
	Attributes.map(AttributeDB.register_attribute)
