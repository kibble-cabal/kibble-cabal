@tool
extends BTPrint

## Prints given string with format params, then xecutes child and returns child's status


func _generate_name() -> String:
	return "Print \"{0}\", then...".format([string])


func _tick(delta: float) -> Status:
	_print()
	
	if get_child_count_excluding_comments() == 0:
		return FAILURE
	
	var child := get_child(0)
	return child.execute(delta)
