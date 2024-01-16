@tool
extends BTComposite

## Executes the second child only if the first child succeeds. Otherwise, executes third child.

var condition_status: Status = FRESH


func _generate_name() -> String:
	var extra_string := "Otherwise, execute child 3." if get_child_count_excluding_comments() > 2 else ""
	return "If child 1 succeeds, execute child 2." + extra_string


func _get_configuration_warning() -> PackedStringArray:
	var warning := PackedStringArray()
	if not get_child_count_excluding_comments() in [2, 3]:
		warning.append("Should have exactly 2 or 3 children!")
	return warning


func _tick(delta: float) -> Status:
	if get_child_count_excluding_comments() < 2:
		return FAILURE
	
	if condition_status not in [SUCCESS, FAILURE]:
		condition_status = get_child(0).execute(delta)
		if condition_status == RUNNING:
			return RUNNING
	
	match condition_status:
		SUCCESS: return get_child(1).execute(delta)
		FAILURE when get_child_count_excluding_comments() > 2:
			return get_child(2).execute(delta)
	
	return FAILURE
