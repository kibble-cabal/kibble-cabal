
@tool
extends BTActivateAbility

@export var ability: AAbility


func _get_configuration_warning() -> PackedStringArray:
	var warning := PackedStringArray()
	if not ability: warning.append("Ability not provided!")
	return warning


func get_ability() -> AAbility:
	return ability
