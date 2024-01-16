@tool
extends BTAbilitySystemAction


@export var ability: AAbility
@export var stage: AAbilityStage


func _generate_name() -> String:
	if ability and stage: return "Check ability \"{0}\" is in stage \"{1}\"".format([ability.name, stage.name])
	return "Check ability stage"


func _get_configuration_warning() -> PackedStringArray:
	var warning := super()
	if not ability: warning.append("Ability not provided!")
	if not stage: warning.append("Ability stage not provided!")
	return warning


func _tick(_delta: float) -> Status:
	var node := get_ability_system()
	if node and ability and stage:
		var tasks = node.state.ability_tasks.filter(
			func(task: AAbilityTask) -> bool:
				return task.ability == ability
		)
		if tasks.size() and tasks.any(
			func(task: AAbilityTask) -> bool:
				return task.stage == stage
		): return SUCCESS
	return FAILURE
