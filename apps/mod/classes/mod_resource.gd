class_name ModResource extends ModdableResource

@export var zip_path: String
@export var id: String
@export var author: String
@export var link: String
@export var version: String
@export var icon: Texture2D
@export_multiline var display_description: String
@export var display_name: String
@export var entry_script_path: String


func run_entry_script() -> void:
	if zip_path.is_empty() or entry_script_path.is_empty(): return
	
	# Get entry script contents as string
	var loader := ContentLoader.new_zip(zip_path)
	var string := loader.load_string(entry_script_path)
	loader.close_zip()
	
	# Do string in new sandboxed Lua envionrment
	var lua := LuaSystem.create_environment()
	var error: LuaError = lua.do_string(string)
	if error: print_rich(Bb.yellow("Lua Error ({0}): {1}".format([error.type, error.message])))
