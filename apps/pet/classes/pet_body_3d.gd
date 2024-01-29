class_name PetBody3D extends CharacterBody3D

signal move_started
signal move_finished

@export var navigation_agent: NavigationAgent

## If provided, This ray updates every frame (while moving) to point in the direction the character is facing
@export var facing_ray: RayCast3D

var is_moving: bool = false:
	set(value):
		match [is_moving, value]:
			[false, true]: move_started.emit()
			[true, false]: move_finished.emit()
		is_moving = value

var _facing_ray_length: float


func _ready() -> void:
	navigation_agent.navigation_finished.connect(func(): move_finished.emit())
	
	if facing_ray: _facing_ray_length = facing_ray.target_position.length()


func _physics_process(_delta: float) -> void:
	is_moving = not [velocity.x, velocity.y, velocity.z].all(func(num): return is_zero_approx(num))
	if is_moving:
		if facing_ray:
			facing_ray.target_position = (Vector3(_facing_ray_length, _facing_ray_length, _facing_ray_length) * velocity.sign()).limit_length(_facing_ray_length)
		move_and_slide()


func lua_fields() -> Array:
	return ["is_moving", "velocity"]
