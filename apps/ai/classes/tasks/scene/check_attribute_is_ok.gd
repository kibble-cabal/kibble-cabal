@tool
extends BTAction

@export var attribute: AAttribute
@export var ability_system: BBNode


func _generate_name() -> String:
	return "Check attribute \"{0}\" is OK".format([attribute.name])


func _get_configuration_warning() -> PackedStringArray:
	var warning := PackedStringArray()
	if not attribute: warning.append("Missing attribute!")
	if not ability_system: warning.append("Missing ability system!")
	return warning


func _tick(_delta: float) -> Status:
	if not attribute or not ability_system: return FAILURE
	
	var node := ability_system.get_value(agent, blackboard) as AbilitySystemComponent
	if not node: return FAILURE
	
	var table := node.get_table_with_attribute(attribute)
	if not table: return FAILURE
	
	if table.is_ok(attribute): return SUCCESS
	else: return FAILURE
