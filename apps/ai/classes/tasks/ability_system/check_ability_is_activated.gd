@tool
extends BTAbilitySystemAction


@export var ability: Ability:
	set(value):
		ability = value
		emit_changed()


func _generate_name() -> String:
	if ability: return "Check {0} is activated".format([ability])
	return "Check ability is activated"


func _get_configuration_warning() -> PackedStringArray:
	var warning := super()
	if not ability: warning.append("Ability not provided!")
	return warning


func _tick(_delta: float) -> Status:
	var node := get_ability_system()
	if node and ability:
		var events = node.events.filter(
			func(event: AbilityEvent) -> bool:
				return event.ability == ability
		)
		if events.size():
			return SUCCESS
	return FAILURE
