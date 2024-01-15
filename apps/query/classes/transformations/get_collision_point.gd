class_name GetCollisionPointQueryTransformation extends QueryTransformation


func transform(results: Array):
	return results.map(_transform_item)


func _transform_item(item: PhysicsQueryResult) -> Vector2:
	return item.collision_point
