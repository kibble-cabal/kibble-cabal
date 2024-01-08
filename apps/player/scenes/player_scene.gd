extends PlayerBody2D

var resource: PlayerResource

@onready var body_sprite := $BodySprite as Sprite2D
@onready var collision_shape := $CollisionShape as CollisionShape2D
@onready var footstep_player := $FootstepPlayer as SoundEffectPlayer2D

func set_resource(resource_value: PlayerResource) -> void:
	resource = resource_value
	reset()


func reset() -> void:
	speed = 144
	target_margin = 0
	size = Vector2(32, 64)
	motion_mode = CharacterBody2D.MOTION_MODE_FLOATING
	
