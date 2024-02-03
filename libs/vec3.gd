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
