class_name FilterByDistanceQueryFilter extends QueryFilter

enum Op {
	LESS_THAN,
	LESS_THAN_OR_EQUAL_TO,
	EQUAL,
	GREATER_THAN,
	GREATER_THAN_OR_EQUAL_TO
}

@export var operator: Op = Op.LESS_THAN
@export var distance: int = 0


func filter(results: Array) -> Array:
	return results.filter(_filter_item)


func _filter_item(item) -> bool:
	if not item is PhysicsQueryResult: return false
	match operator:
		Op.LESS_THAN: return item.distance < distance
		Op.LESS_THAN_OR_EQUAL_TO: return item.distance <= distance
		Op.GREATER_THAN: return item.distance > distance
		Op.GREATER_THAN_OR_EQUAL_TO: return item.distance >= distance
		Op.EQUAL: return item.distance == distance
	return false
	
