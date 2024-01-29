class_name PhysicsQuery extends Query

@export var region: Shape3D
@export_flags_2d_physics var collision_mask = 0
@export var detect_bodies: bool = true
@export var detect_areas: bool = false


func search(caller: Node3D) -> Array[PhysicsQueryResult]:
	var cast := _make_shape_cast()
	caller.add_child(cast)
	cast.force_shapecast_update()
	var colliders: Array[PhysicsQueryResult] = []
	for index in cast.get_collision_count():
		var point := cast.get_collision_point(index)
		colliders.append(PhysicsQueryResult.new({
			collider = cast.get_collider(index),
			distance = point.distance_to(caller.position),
			collision_point = point,
		}))
	caller.remove_child(cast)
	cast.queue_free()
	return colliders


func _make_shape_cast() -> ShapeCast3D:
	var cast := ShapeCast3D.new()
	cast.shape = region
	cast.target_position = Vector3.ZERO
	cast.collision_mask = collision_mask
	cast.collide_with_bodies = detect_bodies
	cast.collide_with_areas = detect_areas
	return cast
