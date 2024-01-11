class_name Query extends Resource

@export var filters: Array[QueryFilter] = []
@export var transformations: Array[QueryTransformation] = []

func search(_caller) -> Array:
	return []


func query(caller):
	var results = search(caller)
	for filter in filters:
		results = filter.filter(results)
	for transformation in transformations:
		results = transformation.transform(results)
	return results
