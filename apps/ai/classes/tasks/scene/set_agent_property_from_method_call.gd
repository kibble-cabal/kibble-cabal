@tool
class_name BTSetAgentPropertyFromMethodCall extends BTAction

@export var property: StringName
@export var method: StringName
@export var node: BBNode
@export var args: Array
@export var args_include_delta: bool = false


func _generate_name() -> String:
	return "Set Agent.{property} from {node}.{method}()".format({
		node = node,
		method = method if method else &"???",
		property = property if property else &"???"
	})


func _tick(delta: float) -> Status:
	var node_value: Object = node.get_value(agent, blackboard) as Object
	
	if node_value == null: return FAILURE
	if not node_value.has_method(method): return FAILURE
	if not property in agent: return FAILURE
	var result
	if args_include_delta: 
		var new_args := args.duplicate()
		new_args.insert(0, delta)
		result = node_value[method].callv(new_args)
	else: 
		result = node_value[method].callv(args)
	agent[property] = result
	return SUCCESS
