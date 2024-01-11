@tool
extends BTAction

## Checks if the target position has been reached for a given [NavigationAgent].

@export var navigation_agent: BBNode


func _tick(_delta: float) -> Status:
	var navigation_agent_value = navigation_agent.get_value(agent, blackboard)
	if not navigation_agent_value or not navigation_agent_value is NavigationAgent:
		return FAILURE
	
	if navigation_agent_value.is_target_reached(): return SUCCESS
	else: return FAILURE
