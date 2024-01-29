class_name UILayer extends CanvasLayer

## This class inherits its position from its parent [Node2D].

@export var centered: bool = false
@export var extra_offset: Vector2

@onready var parent: Node3D = get_parent()
@onready var viewport := get_viewport()
@onready var camera := viewport.get_camera_3d()


func _init() -> void:
	follow_viewport_enabled = true


func _process(_delta: float) -> void:
	if parent and is_inside_tree() and visible:
		offset = camera.unproject_position(parent.position) + extra_offset
		if centered and viewport: offset -= Vector2(viewport.size) / 2
