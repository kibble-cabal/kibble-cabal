@tool
extends Node3D


@onready var mesh_instance: MeshInstance3D = $CompoundMesh

var mesh: CompoundMesh:
	get: return mesh_instance.mesh as CompoundMesh if mesh_instance else null


func set_mesh(value: Mesh) -> void:
	if mesh_instance:
		mesh_instance.mesh = value
