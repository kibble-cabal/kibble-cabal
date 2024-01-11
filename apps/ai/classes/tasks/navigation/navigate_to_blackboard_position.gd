@tool
class_name BTNavigateToBlackboardPosition extends BTNavigate

## Sets the target position for the given [NavigationAgent] from the Blackboard.
## If the navigation stops but the target is not reached, this task fails.

@export var variable: StringName


func _generate_name() -> String:
	return "Navigate to Blackboard.{0}".format([variable])


func get_navigation_position() -> Vector2:
	return blackboard.get_var(variable, Vector2.ZERO)
