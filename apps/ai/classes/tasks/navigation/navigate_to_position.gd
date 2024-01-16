@tool
class_name BTNavigateToPosition extends BTNavigate

## Sets the target position for the given [NavigationAgent].
## If the navigation stops but the target is not reached, this task fails.

@export var target_position: BBVector2


func _generate_name() -> String:
	return "Navigate to {0}".format([target_position])


func get_navigation_position() -> Vector2:
	return target_position.get_value(agent, blackboard)
