class_name Vec3


static func from(vec2: Vector2, zero_value: float = 0, zero_axis := Vector3.AXIS_Y) -> Vector3:
	match zero_axis:
		Vector3.AXIS_X: return Vector3(zero_value, vec2.x, vec2.y)
		Vector3.AXIS_Y: return Vector3(vec2.x, zero_value, vec2.y)
		Vector3.AXIS_Z: return Vector3(vec2.x, vec2.y, zero_value)
	return Vector3(vec2.x, zero_value, vec2.y)


static func project_position_to_floor(camera: Camera3D, pos: Vector2) -> Vector3:
	var origin := camera.project_ray_origin(pos)
	var direction := camera.project_ray_normal(pos)
	var distance := -origin.y / (direction.y if direction.y != 0 else 1.0)
	return origin + direction * distance


static func move_away(p1: Vector3, p2: Vector3, amount: float) -> Vector3:
	var diff := p1.move_toward(p2, amount) - p1
	return p1 - diff


static func perpendicular(p: Vector3) -> Vector3:
	return Vector3(p.z, p.y, -p.x)


static func get_planes(mesh: Mesh) -> Array[Plane]:
	var planes: Array[Plane] = []
	var faces := mesh.get_faces()
	for i in range(0, faces.size(), 3):
		planes.append(Plane(faces[i], faces[i + 1], faces[i + 2]))
	return planes


## Returns a [PackedVector3Array] containing the three vertices that make up the face closest to
## local_pos on the provided mesh.
static func get_closest_face_on_mesh(mesh: Mesh, local_pos: Vector3) -> PackedVector3Array:
	var face := PackedVector3Array()
	var dist := INF
	var closest_face: int = -1
	var faces := mesh.get_faces()
	for i in range(0, faces.size(), 3):
		var dist1 := absf(faces[i].distance_to(local_pos))
		var dist2 := absf(faces[i + 1].distance_to(local_pos))
		var dist3 := absf(faces[i + 2].distance_to(local_pos))
		var current_dist := dist1 + dist2 + dist3
		if current_dist < dist:
			dist = current_dist
			closest_face = i
	if closest_face >= 0:
		face.append_array([faces[closest_face], faces[closest_face + 1], faces[closest_face + 2]])
	return face


## Clamps the return value of [method Geometry3D.get_triangle_barycentric_coords] to only return points within the triangle.
## Taken from [url=https://stackoverflow.com/questions/14467296/barycentric-coordinate-clamping-on-3d-triangle]this StackOverflow answer[/url].
static func clamp_barycentric_coords(coords: Vector3, p0: Vector3, p1: Vector3, p2: Vector3) -> Vector3:
	var clamp01 := func(val: float) -> float: return clampf(val, 0, 1)
	var p := p0 * coords.x + p1 * coords.y + p2 * coords.z
	if coords.x < 0:
		var t = (p - p1).dot(p2 - p1) / (p2 - p1).dot(p2 - p1)
		t = clamp01.call(t)
		return Vector3(0, 1 - t, t)
	if coords.y < 0:
		var t = (p - p2).dot(p0 - p2) / (p0 - p2).dot(p0 - p2)
		t = clamp01.call(t)
		return Vector3(t, 0, 1 - t)
	if coords.z < 0:
		var t = (p - p0).dot(p1 - p0) / (p1 - p0).dot(p1 - p0)
		t = clamp01.call(t)
		return Vector3(1 - t, t, 0)
	return coords


## Returns the nearest position within the nearest face of the mesh to local_pos.
## This can be used for snapping points to mesh surfaces.
static func get_closest_point_on_mesh(mesh: Mesh, local_pos: Vector3) -> Vector3:
	var dist := INF
	var face := get_closest_face_on_mesh(mesh, local_pos)
	if face.size():
		var weights := clamp_barycentric_coords(
			Geometry3D.get_triangle_barycentric_coords(local_pos, face[0], face[1], face[2]),
			face[0], face[1], face[2]
		)
		return weights.x * face[0] + weights.y * face[1] + weights.z * face[2]
	return local_pos


## Returns the vertex on mesh closest to local_pos.
static func get_closest_vertex_on_mesh(mesh: Mesh, local_pos: Vector3) -> Vector3:
	var dist := INF
	var closest_vertex: Vector3 = local_pos
	for vert in mesh.get_faces():
		var current_dist := local_pos.distance_to(vert)
		if current_dist < dist:
			closest_vertex = vert
			dist = current_dist
	return closest_vertex


## Draws the provided mesh wireframe with the provided transformation.
static func debug_draw_mesh(mesh: Mesh, transform := Transform3D.IDENTITY, size := 0.01, color := Color.RED) -> void:
	var points := mesh.get_faces()
	for i in range(0, points.size(), 3):
		DebugDraw3D.draw_point_path([
			points[i] * transform.affine_inverse(),
			points[i + 1] * transform.affine_inverse(),
			points[i + 2] * transform.affine_inverse(),
			points[i] * transform.affine_inverse()
		], DebugDraw3D.POINT_TYPE_SPHERE, size, color, color * 0.75)
