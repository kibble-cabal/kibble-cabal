class_name ExpansionPackLoader

## This class handles discovery and opening of expansion packs.

const DIRS_TO_SEARCH: Array[String] = ["res://", "user://"]
const DIRS_TO_SKIP: Array[String] = ["res://addons", "res://apps", "res://content"]

var content_loader := ContentLoader.new()


func _init() -> void:
	content_loader.ignored_dirs = DIRS_TO_SKIP


func load_packs(verbose: bool = true) -> Array[ExpansionPackResource]:
	if verbose: Log.start_section(self)
	
	# Discover *.expansionpack.pck files
	var discovered_pck_files := PackedStringArray()
	for dir in DIRS_TO_SEARCH:
		discovered_pck_files.append_array(content_loader.get_files_by_extension(dir, [
			"expansionpack.pck",
			"expansionpack.zip"
		]))
	
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
		discovered_resource_files.append_array(content_loader.get_files_by_extension(dir, [
			"expansionpack.tres",
			"expansionpack.res"
		]))
	
	if verbose: Log.bullet("Discovered resource files: {0}".format([discovered_resource_files]))
	
	# Load discovered [ExpansionPackResource] files
	var pack_resources: Array[ExpansionPackResource] = []
	for file in discovered_resource_files:
		var resource := content_loader.load_resource(file)
		if resource is ExpansionPackResource:
			pack_resources.append(resource)
	
	if verbose: 
		Log.end_section(self, "Finished!")
	
	return pack_resources


func lua_fields() -> Array:
	return ["load_packs"]


func _to_string() -> String:
	return "ExpansionPackLoader"
