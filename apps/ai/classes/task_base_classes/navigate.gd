@tool
class_name BTNavigate extends BTAction

## Sets the target position for the given [NavigationAgent].
## If the navigation stops but the target is not reached, this task fails.
## [b]Note:[/b] This is a base class and should probably not be used on its own.

@export var navigation_agent_path: NodePath

var navigation_agent: NavigationAgent


func _enter() -> void:
	navigation_agent = agent.get_node(navigation_agent_path)
	if navigation_agent:
		navigation_agent.set_target_position(get_navigation_position())


func _tick(_delta: float) -> Status:
	if not navigation_agent: return FAILURE
	if navigation_agent.is_navigation_finished():
		if navigation_agent.is_target_reached():
			return SUCCESS
		return FAILURE
	return RUNNING


func get_navigation_position() -> Vector2:
	return blackboard.get_var("navigation_target", Vector2.ZERO)
