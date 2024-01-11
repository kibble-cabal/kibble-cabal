class_name PlayerBody2D extends CharacterBody2D

signal move_started
signal move_finished

## Movement speed in pixels/sec
@export var speed = 400

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
	navigation_agent.navigation_finished.connect(func(): move_finished.emit())


func _physics_process(_delta: float) -> void:
	if navigation_agent and not navigation_agent.is_navigation_finished(): return
	move_and_slide()


func lua_fields() -> Array[String]:
	return ["is_moving", "current_direction"]
