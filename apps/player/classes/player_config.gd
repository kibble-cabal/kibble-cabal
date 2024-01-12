# PlayerConfig
extends Node

# Sprite

var SpriteScene: PackedScene
var SpriteSize: Vector2

# Sound Effects

var FootstepSoundEffects: Array[FootstepSoundEffect] = []
var FallbackFootstepSoundEffect: AudioStream = null

# Subclasses

class FootstepSoundEffect:
	enum GroundType {
		SOFT,
		CRUNCHY,
		HARD
	}
	var ground_type: GroundType
	var sound: AudioStream
	
	@warning_ignore("shadowed_variable")
	func _init(ground_type: GroundType, sound: AudioStream) -> void:
		self.ground_type = ground_type
		self.sound = sound
