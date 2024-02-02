extends Decal

signal clicked(pos: Vector3)

@export var pulse: bool = true
@export var mouse_offset: Vector2 = Vector2(0, -30)

@onready var viewport := get_viewport()
@onready var camera := viewport.get_camera_3d() if viewport else null

var elapsed_time: float = 0


func _process(delta: float) -> void:
	elapsed_time += delta
	if viewport and camera:
		global_position = Vec3.project_position_to_floor(camera, viewport.get_mouse_position() + mouse_offset)
		if pulse: scale = Vector3.ONE * lerpf(1.0, 0.9, sin(elapsed_time * 2))


func _unhandled_input(event: InputEvent) -> void:
	if event.is_action_pressed("click"):
		clicked.emit(global_position)
