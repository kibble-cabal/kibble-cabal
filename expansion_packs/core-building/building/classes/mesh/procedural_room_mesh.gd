@tool
class_name ProceduralRoomMesh extends ArrayMesh

@export var closed: bool = true
@export var points: PackedVector2Array
@export var floor_thickness: float = 0.1
@export var wall_height: float = 1.0
@export var wall_thickness: float = 0.1

@export_group("Floor Materials", "material_floor_")
@export var material_floor_top: BaseMaterial3D
@export var material_floor_sides: BaseMaterial3D

@export_group("Wall Materials", "material_wall_")
@export var material_wall_exterior: BaseMaterial3D
@export var material_wall_interior: BaseMaterial3D
@export var material_wall_tops: BaseMaterial3D

var floor_mesh := ProceduralPolygonMesh.new()


func generate() -> void:
	clear_surfaces()
	
	if points.size() < 3: return
	var polygon := points.duplicate()
	
	# Close polygon
	if closed and not points[-1].is_equal_approx(points[0]):
		polygon.append(polygon[0])
	
	# Generate floor mesh
	floor_mesh.points = polygon
	floor_mesh.thickness = floor_thickness
	floor_mesh.material_top = material_floor_top
	floor_mesh.material_sides = material_floor_sides
	
	# Generate wall meshes
	var wall_meshes: Array[ProceduralWallMesh] = []
	for i in range(polygon.size() - 1):
		var mesh := ProceduralWallMesh.new()
		mesh.thickness = wall_thickness
		mesh.height = wall_height
		mesh.point_1 = Vector3(polygon[i].x, 0, polygon[i].y)
		mesh.point_2 = Vector3(polygon[i + 1].x, 0, polygon[i + 1].y)
		mesh.point_1_bezel_amount = wall_thickness
		mesh.point_2_bezel_amount = wall_thickness
		mesh.outside_material = material_wall_exterior
		mesh.inside_material = material_wall_interior
		mesh.top_material = material_wall_tops
		mesh.edge_material = material_wall_interior
		mesh.generate()
		wall_meshes.append(mesh)
	
	# Combine all meshes
	for _mesh in [floor_mesh] + wall_meshes:
		var mesh := _mesh as ArrayMesh
		for surface_index in range(mesh.get_surface_count()):
			var arrays := mesh.surface_get_arrays(surface_index)
			add_surface_from_arrays(Mesh.PRIMITIVE_TRIANGLES, arrays)
			surface_set_material(get_surface_count() - 1, mesh.surface_get_material(surface_index))
