class_name Path extends Object

## Utilities to help with file system paths
##
## Originally from https://github.com/audse/mod-system/blob/main/addons/mod_system/utils/path.gd

## A [RegEx] that captures three groups from a file path (e.g. [code]my_mod.mod.json[/code]):
## [br]1. [code]ext[/code] - the full extension (e.g. [code]mod.json[/code])
## [br]2. [code]sub_ext[/code] - the sub extension, if it exists (e.g. [code]mod[/code])
## [br]3. [code]main_ext[/code] - the main extension (e.g. [code]json[/code])
static var ExtensionRegEx := RegEx.create_from_string("\\.(?<ext>((?<sub_ext>[^.\\/]+)\\.)?(?<main_ext>[^\\/.]+))$")


## Returns the [b]full[/b] extension of the given path.
## [br]Examples:
## [codeblock]
## assert(Path.get_extension("my_mod.mod.json") == "mod.json")
## assert(Path.get_extension("my_mod.tres") == "tres")
## [/codeblock]
static func get_extension(path: String) -> String:
	var result := Path.ExtensionRegEx.search(path.to_lower())
	if result: return result.get_string(result.names.get("ext"))
	else: return path.get_extension()


## Returns a new path with extension [code].import.tres[/code]
static func to_import_path(path: String) -> String:
	return path.replace(path.get_extension(), "import.tres").simplify_path()


static func has_extension(path: String, extension: String) -> bool:
	return path.to_lower().ends_with(extension.to_lower())


static func has_any_extension(path: String, extensions: Array[String]) -> bool:
	return extensions.any(func(ext): return has_extension(path, ext))
