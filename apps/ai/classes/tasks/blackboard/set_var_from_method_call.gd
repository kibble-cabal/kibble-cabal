@tool
class_name BTSetVarFromMethodCall extends BTAction

@export var variable: StringName
@export var method: StringName
@export var node: BBNode
@export var args: Array
@export var args_include_delta: bool = false


func _generate_name() -> String:
	return "Set Blackboard.{variable} from {node}.{method}()".format({
		method = method if method else &"???",
		variable = variable if variable else &"???",
		node = node
	})


func _tick(delta: float) -> Status:
	var node_value: Object = node.get_value(agent, blackboard) as Object
	if node_value == null: return FAILURE
	if not node_value.has_method(method): return FAILURE
	var result
	if args_include_delta: 
		var new_args := args.duplicate()
		new_args.insert(0, delta)
		result = node_value[method].callv(new_args)
	else: result = node_value[method].callv(args)
	blackboard.set_var(variable, result)
	return SUCCESS
