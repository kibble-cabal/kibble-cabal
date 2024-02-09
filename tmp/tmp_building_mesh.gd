@tool
extends Node3D


@export var regenerate_outer_mesh: bool:
	set(value): generate_outer_mesh()

@onready var inner_mesh_instance := $InnerMesh as MeshInstance3D
@onready var outer_mesh_instance := $OuterMesh as MeshInstance3D


func _ready() -> void:
	generate_outer_mesh()
	pass


func generate_outer_mesh() -> void:
	if not outer_mesh_instance or not inner_mesh_instance: return
	var inner_mesh := inner_mesh_instance.mesh as ExtrudePointsMesh
	if inner_mesh:
		var new_polygons := Geometry2D.offset_polygon(inner_mesh.points, 0.1)
		if not new_polygons.is_empty():
			var outer_mesh := ExtrudePointsMesh.new()
			outer_mesh.direction = inner_mesh.direction
			outer_mesh.flip = not inner_mesh.flip
			outer_mesh.length = inner_mesh.length
			outer_mesh.points = new_polygons[0]
			outer_mesh_instance.mesh = outer_mesh
