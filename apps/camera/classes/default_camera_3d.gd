extends Camera3D

const Pan := {
	MOUSE = "click",
	LEFT = "left",
	RIGHT = "right",
	DOWN = "down",
	UP = "up"
}
const Rotate := {
	MOUSE = "two_finger_click",
	LEFT = "rotate_camera_left",
	RIGHT = "rotate_camera_right"
}
const Zoom := {
	IN = "zoom_in",
	OUT = "zoom_out"
}
const PanActions := [Pan.MOUSE, Pan.LEFT, Pan.RIGHT, Pan.DOWN, Pan.UP]
const RotateActions := [Rotate.MOUSE, Rotate.LEFT, Rotate.RIGHT]
const ZoomActions := [Zoom.IN, Zoom.OUT]

enum Mode { PAN, ROTATE, ZOOM }

@export_range(0, 10, 0.01) var sensitivity : float = 3
@export var max_speed : float = 1000
@export var min_speed : float = 0.2

var is_pressed := Toggle.new(false)
var mode := Mode.PAN

@onready var target_position: Vector3 = global_position
@onready var target_rotation: Quaternion = quaternion
@onready var target_zoom: float = size if projection == PROJECTION_ORTHOGONAL else fov


func _unhandled_input(event: InputEvent) -> void:
	if not current or get_viewport().is_input_handled():
		return
	
	if event is InputEventMouseMotion and is_pressed.is_true():
		match mode:
			Mode.PAN: _pan(event.relative)
			Mode.ROTATE: _rotate(event.relative.normalized())
			Mode.ZOOM: pass
	
	if PanActions.any(event.is_action):
		mode = Mode.PAN
	
	if RotateActions.any(event.is_action):
		mode = Mode.ROTATE
	
	if ZoomActions.any(event.is_action):
		mode = Mode.ZOOM
	
	if (PanActions + RotateActions + ZoomActions).any(event.is_action):
		is_pressed.to(event.is_pressed())


func _input(event: InputEvent) -> void:
	if event is InputEventMouseButton and not event.pressed:
		is_pressed.to(false)


func _process(_delta: float) -> void:
	if not current:
		return
	
	if is_pressed.is_true():
		var direction := Input.get_vector(Pan.RIGHT, Pan.LEFT, Pan.DOWN, Pan.UP)
		if not direction.is_zero_approx():
			_pan(direction)
		
		if Input.is_action_pressed(Rotate.LEFT):
			_rotate(Vector2(1, 0))
		
		if Input.is_action_pressed(Rotate.RIGHT):
			_rotate(Vector2(-1, 0))
		
		if Input.is_action_pressed(Zoom.IN):
			_zoom(1)
		
		if Input.is_action_pressed(Zoom.OUT):
			_zoom(-1)
	
	global_position = global_position.lerp(target_position, 0.05)
	quaternion = quaternion.slerp(target_rotation, 0.1)
	match projection:
		PROJECTION_ORTHOGONAL: size = lerpf(size, target_zoom, 0.1)
		_: fov = lerp_angle(fov, target_zoom, 0.1)


func _pan(direction: Vector2) -> void:
	target_position = global_position + (-Vec3.from(direction.normalized()) * 0.5 * sensitivity) * Transform3D.IDENTITY.rotated(Vector3.MODEL_TOP, rotation.y).affine_inverse()


func _rotate(direction: Vector2) -> void:
	var target_transform := (transform
		.rotated(Vector3(1, 0, 0), deg_to_rad(direction.y) * 1.5 * sensitivity)
		.rotated(Vector3(0, 1, 0), deg_to_rad(direction.x * 1.5 * sensitivity)))
	target_rotation = target_transform.basis.get_rotation_quaternion()


func _zoom(relative: float) -> void:
	match projection:
		PROJECTION_ORTHOGONAL: target_zoom = size + relative / 30 * sensitivity * -1
		_: target_zoom = fov + relative * sensitivity * -1
