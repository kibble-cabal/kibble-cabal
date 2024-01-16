@tool
class_name BTAbilitySystemAction extends BTAction

@export var ability_system: BBNode


func _get_configuration_warning() -> PackedStringArray:
	var warning := PackedStringArray()
	if not ability_system: warning.append("Missing ability system!")
	return warning


func get_ability_system() -> AbilitySystemComponent:
	if ability_system:
		var node = ability_system.get_value(agent, blackboard)
		if node and node is AbilitySystemComponent:
			return node
	return null
