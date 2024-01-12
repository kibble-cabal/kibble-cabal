# ModSystem
extends Node

## List of mod IDs that have been initialized
var initialized_mods: Array[String]


func _ready() -> void:
	Log.start_section(self, "Initializing all registered mods...")
	ModDB.registered_mods.map(initialize)
	Log.end_section(self, "Finished!")
	ModDB.mod_registered.connect(initialize)


func initialize(mod: ModResource) -> void:
	if not mod.id in initialized_mods:
		Log.from(self, "Initializing mod: " + mod.display_name)
		initialized_mods.append(mod.id)
		mod.run_entry_script()


func lua_fields() -> Array:
	return ["initialized_mods"]


func _to_string() -> String:
	return "ModSystem"
