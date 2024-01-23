
@tool
extends BTActivateAbility

@export var ability: Ability:
	set(value):
		ability = value
		emit_changed()


func _get_configuration_warning() -> PackedStringArray:
	var warning := PackedStringArray()
	if not ability: warning.append("Ability not provided!")
	return warning


func get_ability() -> Ability:
	return ability
