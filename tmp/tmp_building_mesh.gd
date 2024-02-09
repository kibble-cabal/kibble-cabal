@tool
extends Node3D


@onready var mesh_instance: MeshInstance3D = $CompoundMesh


var mesh: CompoundMesh:
	get: return mesh_instance.mesh if mesh_instance else null

var exterior_mesh: ExtrudePointsMesh:
	get: return mesh.meshes[0] if mesh else null

var interior_mesh: ExtrudeCurveMesh:
	get: return mesh.meshes[1] if mesh else null

var floor_mesh: CurveMesh:
	get: return mesh.meshes[2] if mesh else null

var top_mesh: PolylineMesh:
	get: return mesh.meshes[3] if mesh else null


func _ready() -> void:
	if not Engine.is_editor_hint():
		mesh_instance.mesh = mesh.duplicate(true)


func set_curve(curve: Curve2D) -> void:
	if interior_mesh: interior_mesh.curve = curve
	if top_mesh: top_mesh.curve = curve
	if floor_mesh: floor_mesh.curve = curve
	generate_exterior_mesh()
	print(curve.point_count)


func generate_exterior_mesh() -> void:
	if interior_mesh and exterior_mesh:
		var new_polygons := Geometry2D.offset_polygon(interior_mesh.points, 0.1)
		if not new_polygons.is_empty():
			exterior_mesh.points = new_polygons[0]
