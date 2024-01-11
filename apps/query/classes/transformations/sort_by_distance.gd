class_name SortByDistanceQueryTransformation extends SortQueryTransformation

enum Direction {
	CLOSEST_FIRST,
	FARTHEST_FIRST
}

@export var direction := Direction.CLOSEST_FIRST


func sort(a: PhysicsQueryResult, b: PhysicsQueryResult) -> bool:
	match direction:
		Direction.CLOSEST_FIRST: return a.distance < b.distance
		Direction.FARTHEST_FIRST: return a.distance > b.distance
	return true
