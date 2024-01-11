@tool
class_name RunQueryTask extends BTAction

@export var node_path: NodePath
@export var query: Query = null
@export var result_var: StringName


func _generate_name() -> String:
	return "RunQueryTask"


func _tick(_delta: float) -> Status:
	var node := agent.get_node(node_path)
	if not node:
		return FAILURE
	var result = query.query(node)
	if not result_var:
		return FAILURE
	blackboard.set_var(result_var, result)
	return SUCCESS
