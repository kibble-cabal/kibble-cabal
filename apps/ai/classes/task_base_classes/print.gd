@tool
class_name BTPrint extends BTDecorator

## Base class to print during a [BTAction].

@export_multiline var string: String
@export var blackboard_format_params: PackedStringArray


func _get_configuration_warning() -> PackedStringArray:
	var warning := PackedStringArray()
	if get_child_count_excluding_comments() == 0: warning.append("Missing child node!")
	return warning


func _print() -> void:
	var format_params := {}
	for param in blackboard_format_params:
		format_params[param] = blackboard.get_var(param)
	print(string.format(format_params))
