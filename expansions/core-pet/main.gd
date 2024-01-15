# core-pet/main.gd

extends Object

const Subtrees = [
	preload("res://expansions/core-pet/content/resources/behavior_trees/fulfill_lowest_need_subtree.tres")
]

const Attributes = [
	preload("res://expansions/core-pet/content/resources/attributes/activity_attribute.tres"),
	preload("res://expansions/core-pet/content/resources/attributes/hunger_attribute.tres"),
	preload("res://expansions/core-pet/content/resources/attributes/thirst_attribute.tres"),
	preload("res://expansions/core-pet/content/resources/attributes/energy_attribute.tres"),
]


func _init() -> void:
	Subtrees.map(SubtreeDB.register)
	Attributes.map(AttributeDB.register_attribute)
