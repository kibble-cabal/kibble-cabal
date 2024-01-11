@tool
class_name BTRunQuery extends BTAction

@export var node_path: NodePath
@export var query: Query = null
@export var result_var: StringName


func _generate_name() -> String:
	var result_string := ", set Blackboard.{0} to result".format([result_var]) if result_var else ""
	if len(query.resource_name):
		return "Run Query \"{0}\"".format([query.resource_name]) + result_string
	if len(query.resource_path): 
		return "Run Query \"{0}\"".format([query.resource_path]) + result_string
	return "Run Query" + result_string


func _tick(_delta: float) -> Status:
	var node := agent.get_node(node_path)
	if not node:
		return FAILURE
	var result = query.query(node)
	if not result_var:
		return FAILURE
	blackboard.set_var(result_var, result)
	return SUCCESS
