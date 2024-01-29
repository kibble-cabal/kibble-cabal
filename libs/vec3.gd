class_name Vec3


static func project_position_to_floor(camera: Camera3D, pos: Vector2) -> Vector3:
	var origin := camera.project_ray_origin(pos)
	var direction := camera.project_ray_normal(pos)
	var distance := -origin.y / (direction.y if direction.y != 0 else 1.0)
	return origin + direction * distance
