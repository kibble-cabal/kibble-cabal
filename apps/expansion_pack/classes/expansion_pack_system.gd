# ExpansionPackSystem
extends Node

signal all_packs_initialized
signal pack_initialized(pack: ExpansionPackResource)

## List of expansion pack IDs that have been initialized
var initialized_expansion_packs: Array[String]


func _init() -> void:
	Log.start_section(self, "Initializing all registered expansion packs...")
	ExpansionPackDB.registered_packs.map(initialize)
	all_packs_initialized.emit()
	Log.end_section(self, "Finished!")
	
	ExpansionPackDB.pack_registered.connect(initialize)


func initialize(pack: ExpansionPackResource) -> void:
	if not pack.id in initialized_expansion_packs:
		Log.from(self, "Initializing expansion pack: " + pack.display_name)
		initialized_expansion_packs.append(pack.id)
		if pack.entry_script:
			pack.entry_script.new()
		pack_initialized.emit(pack)


func lua_fields() -> Array:
	return ["initialized_expansion_packs"]


func _to_string() -> String:
	return "ExpansionPackSystem"
