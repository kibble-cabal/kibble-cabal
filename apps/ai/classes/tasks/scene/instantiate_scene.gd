@tool
extends BTAction

@export var scene: PackedScene
@export var blackboard_property_name: StringName


func _generate_name() -> String:
	return "Instantiate scene {0}".format([scene])


func _get_configuration_warning() -> PackedStringArray:
	var warning := PackedStringArray()
	if not scene: warning.append("Missing scene!")
	return warning


func _tick(_delta: float) -> Status:
	if scene:
		var node = scene.instantiate()
		if blackboard_property_name:
			blackboard.set_var(blackboard_property_name, node)
		agent.add_child(node)
		return SUCCESS
	return FAILURE
