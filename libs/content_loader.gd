class_name ContentLoader extends Resource

## Handles loading pretty much any files
##
## This class is sort of like [FileAccess] + [ResourceLoader] + [ZIPReader]. It handles loading
## resources, images, scripts, JSON, etc with a unified API. This helps to abstract file system
## stuff, as it can get overly complex accounting for all different loading methods.
## [br][br]
## Originally from https://github.com/audse/mod-system/blob/main/addons/mod_system/utils/content_loader.gd

enum Mode {
	FILE, ## The files to be loaded exist in the file system. All loading will be done with [FileAccess] and [ResourceLoader].
	ZIP, ## The files to be loaded exist in a zip file. All loading will be done with [ZIPreader].
}

## Defines the mode that files will be loaded.
var mode: Mode = Mode.FILE

## If not [code]null[/code], will be used to load files inside a ZIP.
var zip_reader: CustomZIPReader = null

var ignored_files: Array[String] = []
var ignored_dirs: Array[String] = []


## Returns a new [ContentLoader] with a new [CustomZIPReader] and [member mode] set to [enum Mode.ZIP].
static func new_zip(path: String) -> ContentLoader:
	var loader := ContentLoader.new()
	loader.open_zip(path)
	return loader


func open_zip(path: String) -> void:
	zip_reader = CustomZIPReader.new()
	zip_reader.open(path)
	mode = Mode.ZIP


func close_zip() -> void:
	if zip_reader: zip_reader.close()
	mode = Mode.FILE
	zip_reader = null


## Returns [code]true[/code] if a file exists at the given [code]path[/code].
func exists(path: String) -> bool:
	match mode:
		Mode.FILE: return FileAccess.file_exists(path)
		Mode.ZIP: return zip_reader.exists(path)
	return false


## Returns the [PackedByteArray] at the given [code]path[/code], if it exists. Otherwise, returns an empty [PackedByteArray].
func load_bytes(path: String) -> PackedByteArray:
	match mode:
		Mode.FILE: return FileAccess.get_file_as_bytes(path)
		Mode.ZIP: return zip_reader.read_file(path)
	return PackedByteArray([])


## Returns a [String] at the given [code]path[/code], if it exists. Otherwise, returns an empty [String].
func load_string(path: String) -> String:
	match mode:
		Mode.FILE: return FileAccess.get_file_as_string(path)
		Mode.ZIP: return zip_reader.read_string(path)
	return ""


## Returns a JSON [Dictionary] at the given [code]path[/code], if it exists. Otherwise, returns [code]null[/code].
func load_json(path: String) -> Dictionary:
	return JSON.parse_string(load_string(path))


## Returns a [Resource] at the given [code]path[/code], if it exists. Otherwise, returns [code]null[/code].
func load_resource(path: String) -> Resource:
	match mode:
		Mode.FILE: 
			if "res://" in path: return load(path)
			return ResourceLoader.load(path)
		Mode.ZIP: return zip_reader.read_resource(path)
	return null


## Returns a [Script] at the given [code]path[/code], if it exists. Otherwise, returns [code]null[/code].
func load_script(path: String) -> Script:
	match mode:
		Mode.FILE: 
			var resource := load_resource(path)
			if resource != null and resource is Script:
				return resource
		Mode.ZIP: return zip_reader.read_script(path)
	return null


## Returns a [Texture2D] at the given [code]path[/code], if it exists. Otherwise, returns [code]null[/code].
func load_image(path: String) -> Texture2D:
	match mode:
		Mode.FILE:
			var resource := load_resource(path)
			if resource != null and resource is Texture2D:
				return resource
		Mode.ZIP: return zip_reader.read_image(path)
	return null


func get_files_filtered(entry_dir: String, filter_func: Callable, recursive: bool = true) -> PackedStringArray:
	match mode:
		Mode.FILE:
			var paths := PackedStringArray()
			
			# Add files within subfolders
			if recursive:
				for dir in DirAccess.get_directories_at(entry_dir):
					var current_path := entry_dir.path_join(dir)
					if not current_path in ignored_dirs:
						paths.append_array(get_files_filtered(current_path, filter_func))
			
			# Add all files in current folder
			for file in DirAccess.get_files_at(entry_dir):
				var current_path := entry_dir.path_join(file)
				if not current_path in ignored_files and filter_func.call(current_path): 
					paths.append(current_path)
			
			return paths
		Mode.ZIP: 
			return zip_reader.get_files_filtered(filter_func)
	return PackedStringArray()


func get_files_by_regex(entry_dir: String, regex: RegEx, recursive: bool = true) -> PackedStringArray:
	return get_files_filtered(
		entry_dir,
		func(path: String) -> bool: return regex.search(path) != null,
		recursive
	)


func get_files_by_extension(entry_dir: String, extensions: Array[String] = [], recursive: bool = true) -> PackedStringArray:
	return get_files_filtered(
		entry_dir,
		func(path: String) -> bool: return Path.has_any_extension(path, extensions),
		recursive
	)
