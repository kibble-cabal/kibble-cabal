# ExpansionPackDB
extends Node


signal pack_registered(pack: ExpansionPackResource)
signal pack_unregistered(pack: ExpansionPackResource)


var registered_packs: Array[ExpansionPackResource] = []

var loader := ExpansionPackLoader.new()


func _init() -> void:
	var packs := loader.load_packs()
	
	Log.start_section(self, "Registering discovered expansion packs...")
	packs.map(register)
	Log.end_section(self, "Finished!")


func register(pack: ExpansionPackResource) -> void:
	Log.from(self, "Registering expansion pack: " + pack.display_name)
	registered_packs.append(pack)
	pack_registered.emit(pack)


func unregister(pack: ExpansionPackResource) -> void:
	registered_packs.erase(pack)
	pack_unregistered.emit(pack)


func find(pack_name: String) -> ExpansionPackResource:
	for pack in registered_packs:
		if pack.name == pack_name: return pack
	return null


func lua_fields() -> Array:
	return ["registered_packs", "find", "loader"]


func _to_string() -> String:
	return "ExpansionPackDB"
