@tool
extends BTPrint

## Executes child. If it succeeds, prints given string with format params. Returns child's status


func _generate_name() -> String:
	return "If success, print \"{0}\"".format([string])


func _tick(delta: float) -> Status:
	if get_child_count_excluding_comments() == 0:
		return FAILURE
	
	var child := get_child(0)
	var child_status := child.execute(delta)
	
	if child_status == SUCCESS: _print()
	
	return child_status
