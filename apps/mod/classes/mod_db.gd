# ModDB
extends Node


signal mod_registered(mod: ModResource)
signal mod_unregistered(mod: ModResource)


var registered_mods: Array[ModResource] = []

var loader := ModLoader.new()


func _ready() -> void:
	var mods := loader.load_mods()
	
	Log.start_section(self, "Registering discovered mods...")
	mods.map(register)
	Log.end_section(self, "Finished!")


func register(mod: ModResource) -> void:
	Log.from(self, "Registering mod: " + mod.id)
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


func _to_string() -> String:
	return "ModDB"
