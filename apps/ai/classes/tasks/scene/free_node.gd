@tool
extends BTAction

@export var node: BBNode


func _generate_name() -> String:
	return "Free Node {0}".format([node])


func _get_configuration_warning() -> PackedStringArray:
	var warning := PackedStringArray()
	if not node: warning.append("Missing node!")
	return warning


func _tick(_delta: float) -> Status:
	if node:
		var node_value: Node = node.get_value(agent, blackboard)
		if node_value and node_value.is_inside_tree() and node_value.is_ready() and not node_value.is_queued_for_deletion():
			node_value.queue_free()
			return SUCCESS
	return FAILURE
