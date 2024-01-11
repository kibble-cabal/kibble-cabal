extends PlayerBody2D

@export var resource: PetResource

@onready var sprite_controller := $SpriteController as SpriteController


func _ready() -> void:
	if resource:
		_instantiate_sprite_controller()
		sprite_controller.modulate = resource.modulate
	super._ready()


func reset() -> void:
	speed = 144
	target_margin = 0
	size = Vector2(32, 64)
	motion_mode = CharacterBody2D.MOTION_MODE_FLOATING


func _instantiate_sprite_controller() -> void:
	move_started.connect(sprite_controller.start.bind("walk"))
	move_finished.connect(sprite_controller.start.bind("default"))

