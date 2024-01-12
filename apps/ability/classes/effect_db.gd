# EffectDB
extends Node


signal effect_registered(effect: AEffect)
signal effect_unregistered(effect: AEffect)


var registered_effects: Array[AEffect] = []


func register(effect: AEffect) -> void:
	registered_effects.append(effect)
	effect_registered.emit(effect)


func unregister(effect: AEffect) -> void:
	registered_effects.erase(effect)
	effect_unregistered.emit(effect)


func find(effect_resource_name: String) -> AEffect:
	for effect in registered_effects:
		if effect.resource_name == effect_resource_name: return effect
	return null


func lua_fields() -> Array[String]:
	return ["registered_effects", "register", "unregister", "find"]
