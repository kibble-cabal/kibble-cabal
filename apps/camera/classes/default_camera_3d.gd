extends Camera3D

enum Mode { NONE, ROTATE, PAN }

@export_range(0, 10, 0.01) var sensitivity : float = 3
@export_range(0, 1000, 0.1) var default_velocity : float = 5
@export_range(0, 10, 0.01) var speed_scale : float = 1.17
@export_range(1, 100, 0.1) var boost_speed_multiplier : float = 3.0
@export var max_speed : float = 1000
@export var min_speed : float = 0.2

var mode := Mode.NONE

func _unhandled_input(event: InputEvent) -> void:
	if not current:
		return
	
	if event is InputEventMouseMotion:
		match mode:
			Mode.PAN: _pan(event.relative)
			Mode.ROTATE:
				if abs(event.relative.y) > abs(event.relative.x):
					_zoom(event.relative.normalized().y)
				else:
					_rotate(event.relative * Vector2(1, 0))
	
	if event.is_action(&"click"):
		mode = Mode.PAN if event.pressed else Mode.NONE
	
	if event.is_action(&"two_finger_click"):
		mode = Mode.ROTATE if event.pressed else Mode.NONE


func _process(_delta: float) -> void:
	if not current:
		return
	
	_pan(Input.get_vector("right", "left", "down", "up"))
	
	if Input.is_action_pressed("rotate_camera_left"):
		_rotate(Vector2(-1, 0))
	
	if Input.is_action_pressed("rotate_camera_right"):
		_rotate(Vector2(1, 0))
	
	if Input.is_action_pressed("zoom_in"):
		_zoom(1)
	
	if Input.is_action_pressed("zoom_out"):
		_zoom(-1)


func _pan(relative: Vector2) -> void:
	var direction := Vector3(-relative.x, relative.y, 0).normalized()
	translate(direction / 300 * sensitivity * default_velocity)


func _rotate(relative: Vector2) -> void:
	rotation.y -= relative.x / 1000 * sensitivity
	rotation.x -= relative.y / 1000 * sensitivity
	rotation.x = clamp(rotation.x, PI / -2, PI / 2)


func _zoom(relative: float) -> void:
	size += relative / 30 * sensitivity * -1
