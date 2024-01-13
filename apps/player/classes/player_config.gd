# PlayerConfig
extends Node

# Physics

var MaxSpeed: float
var CollisionShape: Shape2D
var DetectionRadius: float

# Sprite

## Should extend [SpriteController]
var SpriteScene: PackedScene
var SpriteSize: Vector2

# Sound Effects

var FootstepSoundEffects: Array[FootstepSoundEffect] = []
var FallbackFootstepSoundEffect: AudioStream = null

# Subclasses

class FootstepSoundEffect:
	enum {
		SOFT = 1,
		CRUNCHY = 2,
		HARD = 3
	}
	var ground_type: int
	var sound: AudioStream
	
	@warning_ignore("shadowed_variable")
	func _init(ground_type: int, sound: AudioStream) -> void:
		self.ground_type = ground_type
		self.sound = sound
	
	
	func lua_fields() -> Array:
		return [
			"SOFT",
			"CRUNCHY",
			"HARD",
			"ground_type",
			"sound"
		]


# Methods

func get_footstep_sound(ground_type := FootstepSoundEffect.SOFT) -> AudioStream:
	for i in range(FootstepSoundEffects.size() - 1, -1, -1):
		var sound := FootstepSoundEffects[i]
		if sound.ground_type == ground_type: return sound.sound
	return FallbackFootstepSoundEffect


func lua_fields() -> Array:
	return [
		"MaxSpeed",
		"CollisionShape",
		"DetectionRadius",
		"FallbackFootstepSoundEffect",
		"FootstepSoundEffects",
		"SpriteScene",
		"SpriteSize",
		"get_footstep_sound",
	]
