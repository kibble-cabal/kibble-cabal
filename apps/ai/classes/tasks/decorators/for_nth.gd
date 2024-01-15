@tool
extends BTDecorator

@export var index: int = 0
@export var array_var: BBArray
@export var element_var: String = ""


func _generate_name() -> String:
	return "For item {index} as ${element_var} in {array_var}".format({
		index = index,
		array_var = array_var,
		element_var = element_var
	})


func _get_configuration_warning() -> PackedStringArray:
	var warning := PackedStringArray()
	if index < 0: warning.append("Index must be at least 0!")
	if not array_var: warning.append("Array variable must not be empty!")
	if element_var.is_empty(): warning.append("Element variable must not be empty!")
	if get_child_count_excluding_comments() == 0: warning.append("Must have child task!")
	return warning


func _enter() -> void:
	if array_var and not element_var.is_empty():
		var array = array_var.get_value(agent, blackboard, [])
		if array and array is Array and array.size() > index:
			blackboard.set_var(element_var, array[index])


func _tick(delta: float) -> Status:
	if not can_execute(): return FAILURE
	return get_child(0).execute(delta)


func can_execute() -> bool:
	var array = array_var.get_value(agent, blackboard)
	return (
		array
		and array_var
		and element_var.length() > 0
		and get_child_count_excluding_comments() > 0
		and array is Array
		and array.size() > index
	)
