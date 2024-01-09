extends PlayerBody2D

@onready var collision_shape := $CollisionShape as CollisionShape2D
@onready var footstep_player := $FootstepPlayer as SoundEffectPlayer2D

var sprite_controller: SpriteController

var resource: PlayerResource:
	get: return SaveSystem.current_save.player if SaveSystem and SaveSystem.current_save else null


func _ready() -> void:
	if resource:
		_instantiate_sprite_controller()
	super._ready()


func reset() -> void:
	speed = 144
	target_margin = 0
	size = Vector2(32, 64)
	motion_mode = CharacterBody2D.MOTION_MODE_FLOATING


func _instantiate_sprite_controller() -> void:
	if sprite_controller:
		sprite_controller.queue_free()
		sprite_controller = null
	if resource.sprite_scene:
		sprite_controller = resource.sprite_scene.instantiate()
		move_started.connect(sprite_controller.start.bind("walk"))
		move_finished.connect(sprite_controller.start.bind("default"))
		add_child(sprite_controller)
