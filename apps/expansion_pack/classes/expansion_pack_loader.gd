class_name ExpansionPackLoader

## This class handles discovery and opening of expansion packs.

const DIRS_TO_SEARCH: Array[String] = ["res://", "user://"]
const DIRS_TO_SKIP: Array[String] = ["res://addons", "res://apps", "res://content"]


func load_packs(verbose: bool = true) -> Array[ExpansionPackResource]:
	if verbose: Log.start_section(self)
	# Discover *.expansionpack.pck files
	var discovered_pck_files := PackedStringArray()
	for dir in DIRS_TO_SEARCH:
		discovered_pck_files.append_array(get_files_recursive(dir, filter_pck))
	
	if verbose: Log.bullet("Discovered PCK files: {0}".format([discovered_pck_files]))
	
	# Open discovered pck files
	# See this issue for why this is only in exported builds:
	# https://github.com/godotengine/godot/issues/19815
	if OS.has_feature("standalone"):
		for pack in discovered_pck_files:
			if not ProjectSettings.load_resource_pack(pack):
				push_error("Error opening expansion pack: {0}".format(pack))
	elif verbose: Log.bullet("Can't unpackage PCK files in editor build, skipping")
	
	# Discover *.expansionpack.tres
	var discovered_resource_files := PackedStringArray()
	for dir in ["res://"]:
		discovered_resource_files.append_array(get_files_recursive(dir, filter_resource))
	
	if verbose: Log.bullet("Discovered resource files: {0}".format([discovered_resource_files]))
	
	# Load discovered [ExpansionPackResource] files
	var pack_resources: Array[ExpansionPackResource] = []
	for file in discovered_resource_files:
		var resource := ResourceLoader.load(file, "ExpansionPackResource")
		if resource is ExpansionPackResource:
			pack_resources.append(resource)
	
	if verbose: 
		Log.end_section(self, "Finished!")
	
	return pack_resources


func get_files_recursive(entry_dir: String, filter_func: Callable) -> PackedStringArray:
	var array := PackedStringArray()
	for dir in DirAccess.get_directories_at(entry_dir):
		if not dir in DIRS_TO_SKIP:
			array.append_array(get_files_recursive(entry_dir.path_join(dir), filter_func))
	for file in DirAccess.get_files_at(entry_dir):
		var path := entry_dir.path_join(file)
		if filter_func.call(path): array.append(path)
	return array


static func filter_pck(path: String) -> bool:
	return path.to_lower().ends_with(".expansionpack.pck") or path.to_lower().ends_with(".expansionpack.zip")


static func filter_resource(path: String) -> bool:
	return path.to_lower().ends_with(".expansionpack.tres") or path.to_lower().ends_with(".expansionpack.res")


func lua_fields() -> Array[String]:
	return ["load_packs"]


func _to_string() -> String:
	return "ExpansionPackLoader"
