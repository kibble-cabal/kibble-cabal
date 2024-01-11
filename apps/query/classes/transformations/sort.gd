class_name SortQueryTransformation extends QueryTransformation


func transform(results: Array) -> Array:
	results.sort_custom(sort)
	return results


func sort(_a, _b) -> bool:
	return true
