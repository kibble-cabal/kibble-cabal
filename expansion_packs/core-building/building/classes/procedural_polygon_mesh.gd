@tool
class_name ProceduralPolygonMesh extends ArrayMesh

@export var points: PackedVector2Array:
	set(value):
		points = value
		generate()

@export var thickness: float = 0.1:
	set(value):
		thickness = value
		generate()

@export_group("Materials", "material_")
@export var material_top: BaseMaterial3D:
	set(value):
		material_top = value
		generate()

@export var material_sides: BaseMaterial3D:
	set(value):
		material_sides = value
		generate()

@export var material_bottom: BaseMaterial3D:
	set(value):
		material_bottom = value
		generate()

var surface: SurfaceTool


func get_vertex(point: Vector2) -> Vector3:
	return Plane(Vector3.UP).project(Vector3(point.x, 0, point.y))


func generate_surface(callable: Callable, material: BaseMaterial3D, i: int) -> void:
	surface.begin(Mesh.PRIMITIVE_TRIANGLES)
	surface.set_uv(Vector2.ZERO)
	
	var arrays: Array = callable.call()
	if arrays.size() > 0 and arrays[0] and arrays[0].size() > 0:
		add_surface_from_arrays(Mesh.PRIMITIVE_TRIANGLES, arrays)
	
	if get_surface_count() > i and material:
		surface_set_material(i, material)


func generate_top() -> Array:
	var verts := Geometry2D.triangulate_polygon(points)
	surface.set_normal(Vector3.UP)
	
	for i in verts:
		surface.add_vertex(get_vertex(points[i]) + Vector3(0, thickness, 0))
	
	return surface.commit_to_arrays()


func generate_bottom() -> Array:
	var verts := Geometry2D.triangulate_polygon(points)
	verts.reverse()
	
	surface.set_normal(Vector3.DOWN)
	
	for i in verts: surface.add_vertex(get_vertex(points[i]))
	
	return surface.commit_to_arrays()


func generate_side(p1: int, p2: int) -> Array:
	var a := points[p1]
	var b := points[p2]
	var verts := PackedVector3Array([
		# tri 1
		Vector3(a.x, 0, a.y),
		Vector3(a.x, thickness, a.y),
		Vector3(b.x, 0, b.y),
		# tri 2
		Vector3(a.x, thickness, a.y),
		Vector3(b.x, thickness, b.y),
		Vector3(b.x, 0, b.y)
	])
	
	for vert in verts: surface.add_vertex(vert)
	
	surface.generate_normals()
	surface.generate_tangents()
	
	return surface.commit_to_arrays()


func generate() -> void:
	clear_surfaces()
	if points.size() >= 3:
		surface = SurfaceTool.new()
		generate_surface(generate_top, material_top, 0)
		generate_surface(generate_bottom, material_bottom, 1)
		for i in range(0, points.size()):
			generate_surface(
				generate_side.bind(i - 1, i), 
				material_sides,
				i + 2
			)
