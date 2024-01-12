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

const Quests = [
	preload("res://expansions/core/quests/assets/resources/test_quest_1.tres")
]

const Subtrees = [
	preload("res://expansions/core/ai/assets/resources/test_subtree_resource_1.tres"),
	preload("res://expansions/core/ai/assets/resources/test_subtree_resource_2.tres")
]


func _init() -> void:
	Locations.map(LocationDB.register)
	Settings.map(SettingDefinitionDB.register)
	Items.map(ItemDB.register)
	Quests.map(QuestDB.register)
	Subtrees.map(SubtreeDB.register)
