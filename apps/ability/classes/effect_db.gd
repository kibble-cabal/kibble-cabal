# EffectDB
extends Node


signal effect_registered(effect: Effect)
signal effect_unregistered(effect: Effect)


var registered_effects: Array[Effect] = []


func register(effect: Effect) -> void:
	registered_effects.append(effect)
	effect_registered.emit(effect)


func unregister(effect: Effect) -> void:
	registered_effects.erase(effect)
	effect_unregistered.emit(effect)


func find(identifier: String) -> Effect:
	for effect in registered_effects:
		if effect.identifier == identifier: return effect
	return null


func lua_fields() -> Array:
	return ["registered_effects", "register", "unregister", "find"]
