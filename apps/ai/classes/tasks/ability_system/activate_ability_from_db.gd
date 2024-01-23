@tool
extends BTActivateAbility

@export var ability_identifier: String:
	set(value):
		ability_identifier = value
		emit_changed()


func _get_configuration_warning() -> PackedStringArray:
	var warning := PackedStringArray()
	if len(ability_identifier) == 0: warning.append("Ability identifier can't be empty!")
	return warning


func get_ability() -> Ability:
	return AbilityDB.find(ability_identifier)
