@tool
extends BTAction

@export var node: BBNode
@export var signal_name: StringName


var has_occurred := false


func _generate_name() -> String:
	return "Await signal {0}.{1}".format([
		node.to_string() if node else "???",
		signal_name if signal_name else "???"
	])


func _get_configuration_warning() -> PackedStringArray:
	var warning := PackedStringArray()
	if not node: warning.append("No node provided!")
	if not signal_name: warning.append("No signal name provided!")
	return warning


func _enter() -> void:
	has_occurred = false
	_wait_for_signal()


func _tick(_delta: float) -> Status:
	var node_value = node.get_value(agent, blackboard)
	if not node_value or not signal_name or not node_value.has_signal(signal_name): return FAILURE
	if has_occurred: return SUCCESS
	return RUNNING


func _wait_for_signal() -> void:
	if node and signal_name and not has_occurred:
		var node_value = node.get_value(agent, blackboard)
		if node_value is Node and node_value.has_signal(signal_name):
			await node_value[signal_name]
			has_occurred = true
