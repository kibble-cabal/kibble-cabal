@tool
extends BTAction

## Sets [member element_var] in blackboard to [member array_var][[member index]]

@export var array_var: BBArray
@export var element_var: String
@export var index: int = 0


func _get_configuration_warning() -> PackedStringArray:
	var warning := PackedStringArray()
	if not array_var: warning.append("Missing array variable!")
	if index < 0: warning.append("Index should be above 0!")
	return warning


func _tick(_delta: float) -> Status:
	var array = array_var.get_value(agent, blackboard)
	if not element_var.is_empty() and array and array is Array and array.size() > index:
		blackboard.set_var(element_var, index)
		return SUCCESS
	return FAILURE
