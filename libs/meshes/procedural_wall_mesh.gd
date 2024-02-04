@tool
class_name ProceduralWallMesh extends ArrayMesh

@export var point_1: Vector3
@export var point_2: Vector3
@export var height: float = 1.0
@export var thickness: float = 0.1

@export var outside_material: BaseMaterial3D
@export var inside_material: BaseMaterial3D
@export var bottom_material: BaseMaterial3D
@export var top_material: BaseMaterial3D
@export var edge_material: BaseMaterial3D


## If positive, the inside will be bezeled. 
## If negative, the outside will be bezeled.
@export var point_1_bezel_amount: float = 0.0

## If positive, the inside will be bezeled. 
## If negative, the outside will be bezeled.
@export var point_2_bezel_amount: float = 0.0


func _init() -> void:
	generate()


func get_generated_faces() -> Array[PackedVector3Array]:
	var faces: Array[PackedVector3Array] = []
	
	var height_vec := Vector3(0, height, 0)
	var direction := point_1.direction_to(point_2)
	var inside_delta := Vec3.perpendicular(direction) * thickness
	
	# Bezel point 1
	var point_1_outside := Vec3.move_away(point_1, point_2, point_1_bezel_amount) - inside_delta / 2
	var point_1_inside := point_1 + inside_delta / 2
	
	# Bezel point 2
	var point_2_outside := Vec3.move_away(point_2, point_1, point_2_bezel_amount) - inside_delta / 2
	var point_2_inside := point_2 + inside_delta / 2
	
	# Wall outside
	faces.append(PackedVector3Array([
		# tri 1
		point_1_outside,
		point_1_outside + height_vec,
		point_2_outside + height_vec,
		# tri 2
		point_2_outside + height_vec,
		point_2_outside,
		point_1_outside,
	]))
	
	# Wall inside
	faces.append(PackedVector3Array([
		# tri 1
		point_1_inside,
		point_2_inside + height_vec,
		point_1_inside + height_vec,
		# tri 2
		point_2_inside + height_vec,
		point_1_inside,
		point_2_inside,
	]))
	
	# Bottom
	faces.append(PackedVector3Array([
		# tri 1
		point_1_outside,
		point_2_inside,
		point_1_inside,
		# tri 2
		point_1_outside,
		point_2_outside,
		point_2_inside
	]))
	
	# Top
	faces.append(PackedVector3Array([
		# tri 1
		point_1_outside + height_vec,
		point_1_inside + height_vec,
		point_2_inside + height_vec,
		# tri 2
		point_1_outside + height_vec,
		point_2_inside + height_vec,
		point_2_outside + height_vec
	]))
	
	# Point 1 edge
	faces.append(PackedVector3Array([
		# tri 1
		point_1_outside,
		point_1_inside + height_vec,
		point_1_outside + height_vec,
		# tri 2
		point_1_outside,
		point_1_inside,
		point_1_inside + height_vec,
	]))

	# Point 2 edge
	faces.append(PackedVector3Array([
		# tri 1
		point_2_outside,
		point_2_outside + height_vec,
		point_2_inside + height_vec,
		# tri 2
		point_2_outside,
		point_2_inside + height_vec,
		point_2_inside,
	]))
	
	return faces


func generate() -> void:
	clear_surfaces()
	
	for face in get_generated_faces():
		var surface := SurfaceTool.new()
		surface.begin(Mesh.PRIMITIVE_TRIANGLES)
		surface.set_uv(Vector2.ZERO)
	
		for vert in face:
			surface.add_vertex(vert)
	
		surface.generate_normals()
		surface.generate_tangents()
		
		var arrays := surface.commit_to_arrays()
		if len(arrays) > 0 and arrays[0] and len(arrays[0]) > 0:
			add_surface_from_arrays(Mesh.PRIMITIVE_TRIANGLES, arrays)
	
	var i = 0
	for material in [
		outside_material,
		inside_material,
		bottom_material,
		top_material,
		edge_material,
		edge_material
	]:
		surface_set_material(i, material)
		i += 1
