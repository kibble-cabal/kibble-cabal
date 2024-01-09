# ExpansionPackDB
extends Node


signal pack_registered(pack: ExpansionPackResource)
signal pack_unregistered(pack: ExpansionPackResource)


var registered_packs: Array[ExpansionPackResource] = []


func _ready() -> void:
	discover()


func register(pack: ExpansionPackResource) -> void:
	print("Registering expansion pack: ", pack.display_name)
	registered_packs.append(pack)
	pack_registered.emit(pack)


func unregister(pack: ExpansionPackResource) -> void:
	registered_packs.erase(pack)
	pack_unregistered.emit(pack)


func find(pack_name: String) -> ExpansionPackResource:
	for pack in registered_packs:
		if pack.name == pack_name: return pack
	return null


func discover(entry_dir: String = "res://") -> void:
	for dir in DirAccess.get_directories_at(entry_dir):
		discover(entry_dir.path_join(dir))
	for file in DirAccess.get_files_at(entry_dir):
		if file.ends_with("expansionpack.tres"):
			var resource := ResourceLoader.load(entry_dir.path_join(file))
			if resource is ExpansionPackResource:
				register(resource)


func lua_fields() -> Array[String]:
	return ["registered_packs", "find", "discover"]
