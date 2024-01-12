@tool
class_name BTActivateAbility extends BTAction

## This is a base class, shouldn't be extended directly

@export var ability_system_component: BBNode


func _generate_name() -> String:
	var ability := get_ability()
	if ability: return "Activate \"{0}\" ability".format([ability.name])
	return "Activate ability"


func _get_configuration_warning() -> PackedStringArray:
	return PackedStringArray(["This is a base class, and should not be added directly!"])


func _tick(_delta: float) -> Status:
	var node := ability_system_component.get_value(agent, blackboard) as AbilitySystemComponent
	if node:
		var ability := get_ability()
		if ability:
			if node.activate_ability(ability): return SUCCESS
	return FAILURE


func get_ability() -> AAbility:
	return null
