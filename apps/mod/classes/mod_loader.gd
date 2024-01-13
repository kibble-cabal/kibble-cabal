class_name ModLoader

## This class handles discovery and opening of mods.

const DIRS_TO_SEARCH: Array[String] = ["user://mods"]
const DIRS_TO_SKIP: Array[String] = []


var content_loader := ContentLoader.new()

func _init() -> void:
	content_loader.ignored_dirs = DIRS_TO_SKIP


func load_mods(verbose: bool = true) -> Array[ModResource]:
	if verbose: Log.start_section(self)
	
	# Discover *.zip files
	var discovered_zips := discover_mod_zips()
	
	# Discover *.mod.json within discovered ZIPs
	var mod_resources: Array[ModResource] = []
	for zip in discovered_zips:
		var resource := get_mod_from_zip(zip)
		if resource:
			if verbose: Log.bullet("Discovered mod: {0}".format([resource.id]))
			mod_resources.append(resource)
		else:
			Log.warning("Error loading mod: {0}".format([zip]))
			if verbose: Log.bullet("Most likely, the JSON did not match the schema.")
	
	if verbose: Log.end_section(self, "Finished!")
	
	return mod_resources


func discover_mod_zips() -> PackedStringArray:
	# Discover *.mod.zip files
	var discovered_zips := PackedStringArray()
	for dir in DIRS_TO_SEARCH:
		discovered_zips.append_array(content_loader.get_files_by_extension(dir, [".zip"]))
	
	return discovered_zips


func get_mod_from_zip(zip: String) -> ModResource:
	var mod_resource: ModResource = null
	content_loader.open_zip(zip)
	var resource_paths := content_loader.get_files_by_extension("", ["mod.json"])
	if resource_paths.size():
		var loader := (
			JsonLoader.new()
				.set_output(ModResource.new())
				.set_validator(JsonSchemaLoader.new("res://apps/mod/mod_resource_schema.json").load_validator())
		)
		mod_resource = content_loader.load_json_with_loader(resource_paths[0], loader) as ModResource
		if mod_resource: mod_resource.zip_path = zip
	content_loader.close_zip()
	return mod_resource


func lua_fields() -> Array:
	return ["load_mods"]


func _to_string() -> String:
	return "ModLoader"
