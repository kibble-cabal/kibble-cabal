# ExpansionPackSystem
extends Node


## List of expansion pack IDs that have been initialized
var initialized_expansion_packs: Array[String]


func _ready() -> void:
	Log.start_section(self, "Initializing all registered expansion packs...")
	ExpansionPackDB.registered_packs.map(initialize)
	Log.end_section(self, "Finished!")
	
	ExpansionPackDB.pack_registered.connect(initialize)


func initialize(pack: ExpansionPackResource) -> void:
	if not pack.id in initialized_expansion_packs:
		Log.from(self, "Initializing expansion pack: " + pack.display_name)
		initialized_expansion_packs.append(pack.id)
		if pack.entry_script:
			pack.entry_script.new()


func lua_fields() -> Array:
	return ["initialized_expansion_packs"]


func _to_string() -> String:
	return "ExpansionPackSystem"
