class_name ModResource extends ModdableResource

@export var zip_path: String
@export var id: String
@export var author: String
@export var link: String
@export var version: String
@export var icon_path: String
@export_multiline var display_description: String
@export var display_name: String
@export var entry_script_path: String


func get_icon() -> Texture2D:
	if zip_path.is_empty() or icon_path.is_empty(): return
	return get_zipped(func(loader: ContentLoader): return loader.load_image(icon_path))


func run_entry_script() -> void:
	if zip_path.is_empty() or entry_script_path.is_empty(): return
	
	# Get entry script contents as string
	var string = get_zipped(func(loader: ContentLoader): return loader.load_string(entry_script_path))
	
	# Do string in new sandboxed Lua envionrment
	var lua := LuaSystem.create_environment()
	lua.push_variant("GetCurrentMod", func(): return self) # add this mod to current environment
	var error: LuaError = lua.do_string(string)
	if error: print_rich(Bb.yellow("Lua Error ({0}): {1}".format([error.type, error.message])))


func get_zipped(callable: Callable):
	if zip_path.is_empty(): return null
	var loader := ContentLoader.new_zip(zip_path)
	var result = callable.call(loader)
	loader.close_zip()
	return result


func lua_fields() -> Array:
	return super() + [
		"zip_path",
		"author",
		"id",
		"display_description",
		"display_name",
		"version",
		"link",
		"entry_script_path",
		"icon_path"
	]
