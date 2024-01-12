# AbilityDB
extends Node

signal ability_registered(ability: AAbility)
signal ability_unregistered(ability: AAbility)

signal stage_registered(stage: AAbilityStage)
signal stage_unregistered(stage: AAbilityStage)


var registered_abilities: Array[AAbility] = []
var registered_stages: Array[AAbilityStage] = []


func register_ability(ability: AAbility) -> void:
	registered_abilities.append(ability)
	ability_registered.emit(ability)


func unregister_ability(ability: AAbility) -> void:
	registered_abilities.erase(ability)
	ability_unregistered.emit(ability)


func find_ability(ability_name: String) -> AAbility:
	for ability in registered_abilities:
		if ability.name == ability_name: return ability
	return null


func register_stage(stage: AAbilityStage) -> void:
	registered_stages.append(stage)
	stage_registered.emit(stage)


func unregister_stage(stage: AAbilityStage) -> void:
	registered_stages.erase(stage)
	stage_unregistered.emit(stage)


func find_stage(stage_name: String) -> AAbilityStage:
	for stage in registered_stages:
		if stage.name == stage_name: return stage
	return null


func lua_fields() -> Array:
	return [
		"registered_abilities", 
		"register_ability", 
		"unregister_ability", 
		"find_ability", 
		"registered_stages", 
		"register_stage", 
		"unregister_stage", 
		"find_stage"
	]
