@tool
extends BTAction

## Creates a new action event

@export var action_name: StringName

## If greater than 0, the action will not be released until this amount of time (seconds) has passed
@export var hold_for_seconds: float = 0


func _generate_name() -> String:
	InputMap.load_from_project_settings()
	return "Do action {0}".format([action_name])


func _get_configuration_warning() -> PackedStringArray:
	var warning := PackedStringArray()
	if not InputMap.has_action(action_name):
		warning.append("Action {0} does not exist!".format([action_name]))
	return warning


func _enter() -> void:
	if InputMap.has_action(action_name):
		var press_event := InputEventAction.new()
		press_event.action = action_name
		press_event.pressed = true
		Input.parse_input_event(press_event)


func _exit() -> void:
	if InputMap.has_action(action_name):
		var release_event := InputEventAction.new()
		release_event.action = action_name
		release_event.pressed = false
		Input.parse_input_event(release_event)


func _tick(_delta: float) -> Status:
	if not InputMap.has_action(action_name): 
		return FAILURE
	if elapsed_time < hold_for_seconds:
		return RUNNING
	return SUCCESS
