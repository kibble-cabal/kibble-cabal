@tool
@icon("res://apps/ai/assets/icons/compose.png")
class_name BTPipe extends BTComposite

## Executes all children in order, regardless of whether they succeed or fail.

var last_running_index: int = 0


func _generate_name() -> String:
	return "Pipe"


func _tick(delta: float) -> Status:
	for index in range(last_running_index, get_child_count()):
		if get_child(index).execute(delta) == RUNNING:
			last_running_index = index
			return RUNNING
	return SUCCESS
