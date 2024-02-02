class_name NavigationAgent extends NavigationAgent3D

@export var character: CharacterBody3D


func _ready() -> void:
	velocity_computed.connect(_on_velocity_computed)


func _physics_process(_delta: float) -> void:
	if not character or is_navigation_finished(): return

	var next_path_position := get_next_path_position()
	var new_velocity := character.global_position.direction_to(next_path_position) * max_speed
	if avoidance_enabled:
		set_velocity(new_velocity)
	else:
		_on_velocity_computed(new_velocity)


func _on_velocity_computed(safe_velocity: Vector3) -> void:
	if character:
		character.velocity = safe_velocity


func stop() -> void:
	if character and not is_navigation_finished():
		set_target_position(character.global_position)
		set_velocity(Vector3.ZERO)
