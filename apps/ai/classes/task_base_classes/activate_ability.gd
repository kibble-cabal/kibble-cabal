@tool
class_name BTActivateAbility extends BTAbilitySystemAction


func _generate_name() -> String:
	var ability := get_ability()
	if ability: return "Activate \"{0}\" ability".format([ability.name])
	return "Activate ability"


func _tick(_delta: float) -> Status:
	var node := get_ability_system()
	if node:
		var ability := get_ability()
		if ability and node.activate_ability(ability): 
			return SUCCESS
	return FAILURE


func get_ability() -> AAbility:
	return null
