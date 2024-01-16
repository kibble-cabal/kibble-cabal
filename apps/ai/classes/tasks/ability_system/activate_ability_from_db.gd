@tool
extends BTActivateAbility

@export var ability_name: String


func _get_configuration_warning() -> PackedStringArray:
	var warning := PackedStringArray()
	if len(ability_name) == 0: warning.append("Ability name can't be empty!")
	return warning


func get_ability() -> AAbility:
	return AbilityDB.find_ability(ability_name)
