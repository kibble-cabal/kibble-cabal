class_name PlayerBody2D extends CharacterBody2D

signal move_started
signal move_finished
signal target_received(target_position: Vector2)

## Movement speed in pixels/sec
@export var speed = 400

## Stops when closer than this distance (in pixels) to target. Only applicable for tap-to-move.
@export var target_margin: float = 10

@export var size: Vector2

@export var navigation_agent: NavigationAgent

var is_moving: bool = false:
	set(value):
		match [is_moving, value]:
			[false, true]: move_started.emit()
			[true, false]: move_finished.emit()
		is_moving = value

var current_direction := Vector2i.ZERO:
	set(value):
		current_direction = value
		is_moving = current_direction != Vector2i.ZERO


func _ready() -> void:
	navigation_agent.target_reached.connect(_on_target_reached)


func _unhandled_input(event: InputEvent) -> void:
	if (
		SaveSystem.get_setting("tap_to_move", true)
		and event.is_action_pressed("click") 
		and navigation_agent
	): 
		var mouse_position := get_global_mouse_position()
		navigation_agent.set_target_position(mouse_position)
		target_received.emit(mouse_position)
		move_started.emit()


func _physics_process(_delta: float) -> void:
	if navigation_agent and not navigation_agent.is_navigation_finished(): return
	current_direction = Input.get_vector("left", "right", "up", "down").round()
	velocity = current_direction * speed
	move_and_slide()


func stop() -> void:
	velocity = Vector2.ZERO
	current_direction = Vector2i.ZERO


func max_distance_from_target() -> float:
	return maxf(target_margin + maxf(size.x, size.y) / 4, 1)


func _on_target_reached() -> void:
	move_finished.emit()
