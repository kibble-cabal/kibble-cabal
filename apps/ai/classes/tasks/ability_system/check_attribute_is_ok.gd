@tool
extends BTAbilitySystemAction

@export var attribute: Attribute:
	set(value):
		attribute = value
		emit_changed()


func _generate_name() -> String:
	return "Check attribute \"{0}\" is OK".format([attribute.identifier])


func _get_configuration_warning() -> PackedStringArray:
	var warning := super()
	if not attribute: warning.append("Missing attribute!")
	return warning


func _tick(_delta: float) -> Status:
	var node := get_ability_system()
	
	if not attribute or not node:
		return FAILURE
	
	# FIXME
	# This is temporary, until I add thresholds into new version.
	# Checks if attribute value is above 50%.
	if node.has_attribute(attribute) and node.get_attribute_value(attribute) > (attribute.max_value - attribute.min_value) / 2 + attribute.min_value: 
		return SUCCESS
	
	return FAILURE
