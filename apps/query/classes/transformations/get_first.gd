class_name GetIndexQueryTransformation extends QueryTransformation

@export var index: int = 0


func transform(results: Array):
	return results[index] if results.size() > index else null
