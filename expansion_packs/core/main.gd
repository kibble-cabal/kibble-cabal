class_name CoreExpansionPack

const PauseScene = preload("res://expansion_packs/core/ui/scenes/paused_scene.tscn")
const PlayerSpriteScene = preload("res://expansion_packs/core/player/scenes/player_sprite.tscn")
const DirtFootstepSound = preload("res://expansion_packs/core/player/sounds/sneaker footstep on dirt 01.mp3")

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
	update_player_config()


func update_ui_config() -> void:
	UIConfig.PauseScene = PauseScene


func update_player_config() -> void:
	PlayerConfig.MaxSpeed = 256
	var collision_shape := CapsuleShape2D.new()
	collision_shape.radius = 12
	collision_shape.height = 24
	PlayerConfig.CollisionShape = collision_shape
	PlayerConfig.DetectionRadius = 30
	PlayerConfig.SpriteScene = preload("res://expansion_packs/core/player/scenes/player_sprite.tscn")
	PlayerConfig.SpriteSize = Vector2(32, 64)
	PlayerConfig.FallbackFootstepSoundEffect = DirtFootstepSound
	PlayerConfig.FootstepSoundEffects.append(
		PlayerConfig.FootstepSoundEffect.new(PlayerConfig.FootstepSoundEffect.SOFT, DirtFootstepSound)
	)
