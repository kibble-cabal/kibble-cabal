extends PlayerBody2D

@export var resource: PetResource

@onready var start_position := global_position
@onready var sprite_controller := $SpriteController as SpriteController


func _ready() -> void:
	move_finished.connect(_on_move_finished)
	if resource:
		_instantiate_sprite_controller()
		sprite_controller.modulate = resource.modulate
		global_position = resource.current_position
	super._ready()


func get_random_target() -> Vector2:
	return Vector2(randf_range(300, 800), randf_range(300, 800))


func _instantiate_sprite_controller() -> void:
	move_started.connect(sprite_controller.start.bind("walk"))
	move_finished.connect(sprite_controller.start.bind("default"))


func _on_move_finished() -> void:
	if resource: resource.current_position = global_position
