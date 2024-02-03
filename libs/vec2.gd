class_name Vec2


static func xy(val: float) -> Vector2:
	return Vector2(val, val)


static func from(vec: Vector3, ignore_axis := Vector3.AXIS_Y) -> Vector2:
	match ignore_axis:
		Vector3.AXIS_X: return Vector2(vec.y, vec.z)
		Vector3.AXIS_Y: return Vector2(vec.x, vec.z)
		Vector3.AXIS_Z: return Vector2(vec.x, vec.y)
	return Vector2()


static func to_vec3(vec: Vector2, zero_axis := Vector3.AXIS_Y) -> Vector3:
	match zero_axis:
		Vector3.AXIS_X: return Vector3(0, vec.x, vec.y)
		Vector3.AXIS_Y: return Vector3(vec.x, 0, vec.y)
		Vector3.AXIS_Z: return Vector3(vec.x, vec.y, 0)
	return Vector3()


static func get_perpendicular_direction(p1: Vector2, p2: Vector2) -> Vector2:
	var direction := p1.direction_to(p2)
	return Vector2(direction.y, -direction.x)


static func get_point_on_circle(center: Vector2, radius: float, angle: float) -> Vector2:
	return Vector2(
		center.x + (radius * cos(angle)),
		center.y + (radius * sin(angle))
	)


static func move_away(p1: Vector2, p2: Vector2, amount: float) -> Vector2:
	var diff := p1.move_toward(p2, amount) - p1
	return p1 - diff


static func midpoint(p1: Vector2, p2: Vector2) -> Vector2:
	return p1.lerp(p2, 0.5)
