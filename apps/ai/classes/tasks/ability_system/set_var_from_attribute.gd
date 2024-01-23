@tool
extends BTAbilitySystemAction

@export var attribute: Attribute:
	set(value):
		attribute = value
		emit_changed()

@export var blackboard_var: String:
	set(value):
		blackboard_var = value
		emit_changed()


func _generate_name() -> String:
	return "Set ${0} from attribute \"{1}\"".format([blackboard_var, attribute.name])


func _get_configuration_warning() -> PackedStringArray:
	var warning := super()
	if not attribute: warning.append("Missing attribute!")
	if blackboard_var.is_empty(): warning.append("Missing blackboard variable!")
	return warning


func _tick(_delta: float) -> Status:
	var node := get_ability_system()
	if not attribute or not node or blackboard_var.is_empty():
		return FAILURE
	
	if attribute and node.has_attribute(attribute):
		blackboard.set_var(blackboard_var, node.get_attribute_value(attribute))
		return SUCCESS
	
	return FAILURE
