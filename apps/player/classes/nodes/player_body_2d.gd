class_name PlayerBody2D extends CharacterBody2D

signal move_started
signal move_finished

@export var navigation_agent: NavigationAgent

var is_moving: bool = false:
	set(value):
		match [is_moving, value]:
			[false, true]: move_started.emit()
			[true, false]: move_finished.emit()
		is_moving = value

var current_direction := Vector2i.ZERO


func _ready() -> void:
	navigation_agent.navigation_finished.connect(func(): move_finished.emit())


func _physics_process(_delta: float) -> void:
	is_moving = not (is_zero_approx(velocity.x) and is_zero_approx(velocity.y))
	if is_moving: move_and_slide()


func lua_fields() -> Array[String]:
	return ["is_moving", "current_direction"]
