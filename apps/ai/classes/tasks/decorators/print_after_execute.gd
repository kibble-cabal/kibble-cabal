@tool
extends BTPrint

## Executes child, prints given string with format params, and then returns child's status


func _generate_name() -> String:
	return "Print \"{0}\" after...".format([string])


func _tick(delta: float) -> Status:
	if get_child_count_excluding_comments() == 0:
		return FAILURE
	
	var child := get_child(0)
	var child_status := child.execute(delta)
	
	_print()
	
	return child_status
