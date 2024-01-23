@tool
class_name BTRunQuery extends BTAction

@export var node: BBNode
@export var query: Query = null
@export var result_var: String


func _generate_name() -> String:
	var query_name := (
		query.resource_name if not query.resource_name.is_empty() 
		else query.resource_path if query 
		else "???"
	)
	var result_string := ", set Blackboard.{0} to result".format([result_var]) if result_var else ""
	return "Run Query \"{0}\"{1}".format([query_name, result_string])


func _get_configuration_warning() -> PackedStringArray:
	var warning := PackedStringArray()
	if not node: warning.append("Node is missing!")
	if not query: warning.append("Query is missing!")
	return warning


func _tick(_delta: float) -> Status:
	if not query: return FAILURE
	var node_value = node.get_value(agent, blackboard)
	if not node_value or not node_value is Node:
		return FAILURE
	var result = query.query(node_value)
	if result_var.length():
		blackboard.set_var(result_var, result)
	return SUCCESS
