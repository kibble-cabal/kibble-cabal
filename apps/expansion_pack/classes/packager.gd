class_name Packager

## This class packages directories into *.pck and *.zip files.

class Params:
	var verbose: bool = true
	## If [code]true[/code], will create a backup of the original file when overwriting a file
	var backup_original: bool = true
	## Should include [code]res://[/code].
	## Ex. [code]"res://expansions/core"[/code]
	var entry_dir: String
	var recursive: bool = false
	## May include [code]res://[/code] or [code]user://[/code]. Should NOT include extension.
	## Ex. [code]"core"[/code]
	var output_filename: String
	## The directory that you want the files to be unpackaged within.
	## Ex. [code]"unpacked/core"[/code]
	var unpack_dir: String
	## Should include [code]res://[/code].
	## Ex. [code]["res://expansions/core/some_file.tres"][/code]
	var ignored_files: Array[String] = []
	## Should include [code]res://[/code].
	## Ex. [code]["res://expansions/core/some_dir"][/code]
	var ignored_directories: Array[String] = []
	## Ex. [code]["tres", "ase"][/code]
	var ignored_extensions: Array[String] = []
	
	func quiet() -> Params:
		verbose = false
		return self
	
	func no_backup() -> Params:
		backup_original = false
		return self
	
	func set_recursive(value: bool = true) -> Params:
		recursive = value
		return self
	
	func set_entry_dir(value: String) -> Params:
		entry_dir = value
		return self
	
	func set_output_filename(value: String) -> Params:
		output_filename = value
		return self
	
	func set_unpack_dir(value: String) -> Params:
		unpack_dir = value.replace("res://", "")
		return self
	
	func set_ignored_files(value: Array[String]) -> Params:
		ignored_files = value
		return self
	
	func set_ignored_directories(value: Array[String]) -> Params:
		ignored_directories = value
		return self
	
	func set_ignored_extensions(value: Array[String]) -> Params:
		ignored_extensions = value
		return self


var pck_packer: PCKPacker
var zip_packer: ZIPPacker
var params: Params


func _init(params_value := Params.new()) -> void:
	self.pck_packer = PCKPacker.new()
	self.zip_packer = ZIPPacker.new()
	self.params = params_value


func package() -> void:
	message("⎯⎯⎯⎯⎯", -1, "grey")
	message("[b][Packager][/b]: Creating package [code]\"{0}\"[/code]".format([params.output_filename.get_file()]), -1)

	match handle_existing_pack():
		OK:
			if DirAccess.dir_exists_absolute(params.entry_dir):
				package_pck()
				package_zip()
			else:
				error("Tried to package a directory that doesn't exist! {0}".format([params.entry_dir]))
				return message("Finished with error.", -1)
		ERR_FILE_ALREADY_IN_USE: message("Finished with error (file already in use)", -1)
		_: message("Finished with error.", -1)


func package_pck() -> int:
	var start_result := pck_packer.pck_start(params.output_filename + ".expansionpack.pck")
	if start_result != OK: return start_result
	for_files_in_dir(params.entry_dir, add_file_to_pck)
	return pck_packer.flush(params.verbose)


func package_zip() -> int:
	zip_packer.open(params.output_filename + ".expansionpack.zip")
	for_files_in_dir(params.entry_dir, add_file_to_zip)
	zip_packer.close()
	return OK


## Renames the PCK/ZIP file at [member Params.output_filename] (if it exists) to [code]originalname.backup.pck[/code]
func handle_existing_pack() -> int:
	for file in [params.output_filename + ".expansionpack.pck", params.output_filename + ".expansionpack.zip"]:
		if FileAccess.file_exists(file):
			warning("Existing pack found! It will be removed, but you'll need to replay the scene or reload the editor to create this package")
			if params.backup_original:
				var new_name: String = file.replace(".pck", ".backup.pck")
				message("Backing up existing file to {0}".format([new_name]), 2)
				if DirAccess.rename_absolute(file, new_name) != OK:
					error("Error backing up existing file {0}!".format([file]))
			else:
				message("Deleting existing file", 2)
				if DirAccess.remove_absolute(file) != OK:
					error("Error removing existing file {0}!".format([file]), 3)
			return ERR_FILE_ALREADY_IN_USE
		else:
			message("No existing pack found.")
	return OK


func for_files_in_dir(dir: String, callable: Callable) -> void:
	if params.recursive:
		for child_dir in DirAccess.get_directories_at(dir):
			for_files_in_dir(dir.path_join(child_dir), callable)
	for file in DirAccess.get_files_at(dir):
		callable.call(dir.path_join(file))


func add_file_to_pck(path: String) -> void:
	if path in params.ignored_files or has_ignored_extension(path): return
	pck_packer.add_file(path, path.replace(params.entry_dir, params.unpack_dir))


func add_file_to_zip(path: String) -> void:
	if path in params.ignored_files or has_ignored_extension(path): return
	zip_packer.start_file(path.replace(params.entry_dir, params.unpack_dir))
	zip_packer.write_file(FileAccess.get_file_as_bytes(path))
	zip_packer.close_file()


func has_ignored_extension(path: String) -> bool:
	for extension in params.ignored_extensions:
		if path.ends_with(extension): return true
	return false


func message(string: String, indent: int = 1, color: String = "white") -> void:
	if params.verbose:
		var indent_string = "".lpad(indent * 2) + (" • " if indent != -1 else "")
		print_rich("[color={0}]{1}{2}[/color]".format([color, indent_string, string]))


func warning(string: String, indent: int = 1) -> void:
	message(string, indent, "yellow")
	if not params.verbose: push_warning(string)


func error(string: String, indent: int = 1) -> void:
	message(string, indent, "red")
	push_error(string)
