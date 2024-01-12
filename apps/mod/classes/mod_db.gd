# ModDB
extends Node


signal mod_registered(mod: ModResource)
signal mod_unregistered(mod: ModResource)


var registered_mods: Array[ModResource] = []


func register(mod: ModResource) -> void:
	registered_mods.append(mod)
	mod_registered.emit(mod)


func unregister(mod: ModResource) -> void:
	registered_mods.erase(mod)
	mod_unregistered.emit(mod)


func find(mod_id: String) -> ModResource:
	for mod in registered_mods:
		if mod.id == mod_id: return mod
	return null


func lua_fields() -> Array:
	return ["register", "unregister", "find", "registered_mods"]
