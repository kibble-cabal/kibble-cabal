@tool
extends BTAction

## Succeeds if the provided array has a size that is (at least/at most/exactly) [member size]

enum Op {
	GREATER_THAN,
	GREATER_THAN_OR_EQUAL_TO,
	EQUAL_TO,
	LESS_THAN,
	LESS_THAN_OR_EQUAL_TO
}

@export var array_var: BBArray
@export var size: int = 0
@export var operator := Op.GREATER_THAN


func _generate_name() -> String:
	return "Check size of {array_var} is {operator} {size}".format({
		array_var = array_var,
		operator = _operator_string(),
		size = size
	})


func _get_configuration_warning() -> PackedStringArray:
	var warning := PackedStringArray()
	if not array_var: warning.append("Missing array variable!")
	if size < 0: warning.append("Size should be above 0!")
	return warning


func _tick(_delta: float) -> Status:
	if _is_success(): return SUCCESS
	return FAILURE


func _is_success() -> bool:
	var array = array_var.get_value(agent, blackboard)
	if array and array is Array:
		match operator:
			Op.GREATER_THAN: return array.size() > size
			Op.GREATER_THAN_OR_EQUAL_TO: return array.size() >= size
			Op.EQUAL_TO: return array.size() == size
			Op.LESS_THAN: return array.size() < size
			Op.LESS_THAN_OR_EQUAL_TO: return array.size() <= size
	return false


func _operator_string() -> String:
	return Op.find_key(operator).to_lower().replace("_", " ")
