@tool
extends BTAction

## Checks if the navigation is finished for a given [NavigationAgent].

@export var navigation_agent: BBNode


func _tick(_delta: float) -> Status:
	var navigation_agent_value = navigation_agent.get_value(agent, blackboard)
	if not navigation_agent_value or not navigation_agent_value is NavigationAgent:
		return FAILURE
	
	if navigation_agent_value.is_navigation_finished(): return SUCCESS
	else: return FAILURE
