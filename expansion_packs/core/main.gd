class_name CoreExpansionPack

const PauseScene = preload("res://expansion_packs/core/ui/scenes/paused_scene.tscn")

const GameModes = [
	preload("res://expansion_packs/core/game_mode/resources/live_mode_resource.tres"),
	preload("res://expansion_packs/core/game_mode/resources/live_paused_mode_resource.tres"),
]

const Locations = [
	preload("res://expansion_packs/core/location/resources/island_resource.tres")
]

const Settings = [
	preload("res://expansion_packs/core/settings/resources/reduce_motion.tres"),
	preload("res://expansion_packs/core/settings/resources/tap_to_move.tres"),
]

const Items = [
	preload("res://expansion_packs/core/item/resources/flower.tres"),
	preload("res://expansion_packs/core/item/resources/food_bowl.tres"),
]

const Quests = [
	preload("res://expansion_packs/core/quests/resources/test_quest_1.tres")
]

const Subtrees = [
	preload("res://expansion_packs/core/ai/resources/test_subtree_resource_1.tres"),
	preload("res://expansion_packs/core/ai/resources/test_subtree_resource_2.tres")
]


func _init() -> void:
	GameModes.map(GameModeDB.register)
	Locations.map(LocationDB.register)
	Settings.map(SettingDefinitionDB.register)
	Items.map(ItemDB.register)
	Quests.map(QuestDB.register)
	Subtrees.map(SubtreeDB.register)
	
	update_ui_config()


func update_ui_config() -> void:
	UIConfig.PauseScene = PauseScene
