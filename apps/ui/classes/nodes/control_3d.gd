class_name Control3D extends Control

## This class inherits its position from its parent [Node3D].

@export var local_position: Vector3
@export var center: bool = true

@onready var parent := get_parent()
@onready var viewport := get_viewport()
@onready var camera := viewport.get_camera_3d()


func _process(_delta: float) -> void:
	if is_inside_tree() and visible:
		var total_position = local_position
		if parent is Node3D: total_position += parent.global_position
		position = camera.unproject_position(total_position)
		if center: position -= size / 2
