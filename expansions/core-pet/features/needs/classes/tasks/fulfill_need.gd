@tool
class_name BTFulfillNeed extends BTNavigate

@export var target_item_variable: StringName:
	set(value):
		target_item_variable = value
		emit_changed()

@export var ability_system_node: BBNode:
	set(value):
		ability_system_node = value
		emit_changed()

@export var need_ability: Ability:
	set(value):
		need_ability = value
		emit_changed()

@export var verbose: bool = false

## The event from activating [member need_ability].
var event: AbilityEvent

## If [code]true[/code], navigation to the target item is finished.
var has_navigated := false

## If [code]true[/code], [member need_ability] has finished running.
var has_finished_event := false

## The maximum distance from [member target_item] to consider the target reached.
var max_distance: float = 0


func _generate_name() -> String:
	if not target_item_variable.is_empty():
		return "Fulfill need {0} with item {1}".format([need_ability, target_item_variable])
	else: return "Fulfill need {0}".format([need_ability])


func _get_configuration_warning() -> PackedStringArray:
	var warning := super()
	if not need_ability: warning.append("Missing need ability!")
	if not ability_system_node: warning.append("Missing ability system!")
	return warning


func _enter() -> void:
	super()
	var target_item := get_target_item()
	if target_item: max_distance = Nodes.get_flattened_node_2d_size(target_item).length()


func _tick(delta: float) -> Status:
	# Fail early if any variables are missing.
	var ability_system := get_ability_system()
	if not ability_system or not need_ability: return FAILURE
	
	# If navigation is not finished, continue navigating.
	if get_target_item() and not has_navigated:
		match super(delta):
			SUCCESS:
				has_navigated = true
				return RUNNING
			FAILURE:
				_warning("Unable to navigate to {0}.".format([get_navigation_position()]))
				return FAILURE
			var navigation_status:
				return navigation_status
	
	# If the event is not already started, activate it.
	if not event:
		event = ability_system.activate(need_ability)
		if event: ability_system.ability_event_finished.connect(_on_ability_event_finished)
		else: 
			_warning("Unable to activate {0}.".format([need_ability]))
			return FAILURE
	
	# If the event is finished, the task has succeeded.
	if has_finished_event:
		return SUCCESS
	
	return RUNNING


func get_target_item() -> Node2D:
	var node = blackboard.get_var(target_item_variable)
	if node and node is Node2D:
		return node
	return null


func get_ability_system() -> AbilitySystem:
	var node: Node = ability_system_node.get_value(agent, blackboard)
	if node and node is AbilitySystem:
		return node
	return null


func get_navigation_position() -> Vector2:
	var target_item := get_target_item()
	if target_item: return target_item.global_position
	return Vector2.ZERO


func get_max_distance() -> float:
	return max_distance


func _on_ability_event_finished(finished_event: AbilityEvent) -> void:
	if event and finished_event == event:
		has_finished_event = true
		get_ability_system().ability_event_finished.disconnect(_on_ability_event_finished)


func _warning(message: String, push := false) -> void:
	if verbose: Log.warning_from(_generate_name(), message, push)
