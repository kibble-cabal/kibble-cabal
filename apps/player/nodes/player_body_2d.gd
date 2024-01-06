class_name PlayerBody2D extends CharacterBody2D

## Movement speed in pixels/sec
@export var speed = 400

## Stops when closer than this distance (in pixels) to target. Only applicable for tap-to-move.
@export var target_margin: float = 10

@export var size: Vector2

var current_direction := Vector2i.ZERO

var _target = null

func _unhandled_input(event: InputEvent) -> void:
	if SaveSystem.save.settings.tap_to_move and event.is_action_pressed("click"):
		_target = get_global_mouse_position()


func _physics_process(_delta: float) -> void:
	if _target:
		current_direction = position.direction_to(_target).round()
		velocity = current_direction * speed
		if position.distance_to(_target) < max_distance_from_target():
			stop()
	else:
		current_direction = Input.get_vector("left", "right", "up", "down").round()
		velocity = current_direction * speed
	move_and_slide()


func stop() -> void:
	velocity = Vector2.ZERO
	_target = null
	current_direction = Vector2i.ZERO


func max_distance_from_target() -> float:
	return maxf(target_margin + maxf(size.x, size.y) / 4, 1)
