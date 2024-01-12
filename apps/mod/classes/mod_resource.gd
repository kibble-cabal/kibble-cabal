class_name ModResource extends ModdableResource

@export var id: String
@export var author: String
@export var link: String
@export var version: String
@export var icon: Texture2D
@export_multiline var display_description: String
@export var display_name: String
@export var entry_script_path: String

var entry_script_full_path: String:
	get: return "user://mods".path_join(id).path_join(entry_script_path)


func run_entry_script() -> void:
	var lua := LuaSystem.create_environment()
	#var error: LuaError = LuaSystem.lua.do_file(entry_script_full_path)
	var error: LuaError = lua.do_file(entry_script_path)
	if error: print_rich(Bb.yellow("Lua Error ({0}): {1}".format([error.type, error.message])))
