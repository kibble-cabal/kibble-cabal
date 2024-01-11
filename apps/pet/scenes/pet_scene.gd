extends PlayerBody2D

@export var resource: PetResource

@onready var start_position := global_position
@onready var sprite_controller := $SpriteController as SpriteController

var is_at_target: bool:
	get: 
		if navigation_agent: return navigation_agent.is_navigation_finished()
		else: return true

func _ready() -> void:
	reset()
	if resource:
		_instantiate_sprite_controller()
		sprite_controller.modulate = resource.modulate
	super._ready()


func reset() -> void:
	motion_mode = CharacterBody2D.MOTION_MODE_FLOATING
	if navigation_agent:
		navigation_agent.max_speed = speed
		navigation_agent.movement_speed = speed


func get_random_target() -> Vector2:
	return start_position + Vector2(randf_range(0, 800), randf_range(0, 800))


func _instantiate_sprite_controller() -> void:
	move_started.connect(sprite_controller.start.bind("walk"))
	move_finished.connect(sprite_controller.start.bind("default"))

