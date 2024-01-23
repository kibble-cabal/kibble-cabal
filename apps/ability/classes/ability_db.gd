# AbilityDB
extends Node

signal ability_registered(ability: Ability)
signal ability_unregistered(ability: Ability)

var registered_abilities: Array[Ability] = []


func register(ability: Ability) -> void:
	registered_abilities.append(ability)
	ability_registered.emit(ability)


func unregister(ability: Ability) -> void:
	registered_abilities.erase(ability)
	ability_unregistered.emit(ability)


func find(identifier: String) -> Ability:
	for ability in registered_abilities:
		if ability.identifier == identifier: return ability
	return null


func lua_fields() -> Array:
	return [
		"registered_abilities", 
		"register", 
		"unregister", 
		"find", 
		"registered_stages", 
		"register_stage", 
		"unregister_stage", 
		"find_stage"
	]
