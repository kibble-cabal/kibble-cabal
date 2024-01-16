@tool
extends BTAbilitySystemAction

@export var attribute: AAttribute


func _generate_name() -> String:
	return "Check attribute \"{0}\" is OK".format([attribute.name])


func _get_configuration_warning() -> PackedStringArray:
	var warning := super()
	if not attribute: warning.append("Missing attribute!")
	return warning


func _tick(_delta: float) -> Status:
	var node := get_ability_system()
	
	if not attribute or not node:
		return FAILURE
	
	var table: AAttributeTable = node.get_table_with_attribute(attribute)
	if table and table.is_ok(attribute): 
		return SUCCESS
	
	return FAILURE
