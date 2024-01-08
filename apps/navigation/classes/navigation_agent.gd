class_name NavigationAgent extends NavigationAgent2D

@export var character: CharacterBody2D

## Speed of navigator in pixels/sec
@export var movement_speed: float = 400.0


func _ready() -> void:
	velocity_computed.connect(_on_velocity_computed)


func _physics_process(_delta: float) -> void:
	if not character or is_navigation_finished(): return

	var next_path_position: Vector2 = get_next_path_position()
	var new_velocity: Vector2 = character.global_position.direction_to(next_path_position) * movement_speed
	if avoidance_enabled:
		set_velocity(new_velocity)
	else:
		_on_velocity_computed(new_velocity)


func _on_velocity_computed(safe_velocity: Vector2) -> void:
	if character:
		character.velocity = safe_velocity
		character.move_and_slide()
