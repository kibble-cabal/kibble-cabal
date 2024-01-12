class_name CoreExpansionPack

const PlayerSpriteScene = preload("res://expansions/core/player/scenes/player_sprite.tscn")
const DirtFootstepSound = preload("res://expansions/core/player/assets/sounds/sneaker footstep on dirt 01.mp3")

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
	
	update_player_config()


func update_player_config() -> void:
	PlayerConfig.MaxSpeed = 256
	var collision_shape := CapsuleShape2D.new()
	collision_shape.radius = 12
	collision_shape.height = 24
	PlayerConfig.CollisionShape = collision_shape
	PlayerConfig.DetectionRadius = 30
	PlayerConfig.SpriteScene = preload("res://expansions/core/player/scenes/player_sprite.tscn")
	PlayerConfig.SpriteSize = Vector2(32, 64)
	PlayerConfig.FallbackFootstepSoundEffect = DirtFootstepSound
	PlayerConfig.FootstepSoundEffects.append(
		PlayerConfig.FootstepSoundEffect.new(PlayerConfig.FootstepSoundEffect.SOFT, DirtFootstepSound)
	)
