class_name CoreExpansionPack

const Locations = [
	preload("res://expansions/core/location/island/resources/island_resource.tres")
]

const Settings = [
	preload("res://expansions/core/settings/assets/resources/reduce_motion.tres"),
	preload("res://expansions/core/settings/assets/resources/tap_to_move.tres"),
]

const Items = [
	preload("res://expansions/core/item/assets/resources/flower.tres"),
	preload("res://expansions/core/item/assets/resources/food_bowl.tres"),
]


func _init() -> void:
	Locations.map(LocationDB.register)
	Settings.map(SettingDefinitionDB.register)
	Items.map(ItemDB.register)
