class_name SaveHelper extends Object


const DEFAULT_CONFIG = {
	resource = null,
	base_dir = "user://",
	filename = "",
	binary = false,
	save_on_change = true,
	ignore_resource_path = false,
}

var resource: Resource

## [b]Type: [/b] [String] | [Callable]
var base_dir

## [b]Type: [/b] [String] | [Callable]
## [br]Does not include extension.
var filename

## If [code]false[/code], [member resource] will be saved as a [code].tres[/code] file.
var binary: bool

## If [code]true[/code], [member resource] will be saved every time [signal Resource.changed] is emitted.
var save_on_change: bool

## If [code]false[/code], [member resource] will be saved at it's [member Resource.resource_path] when available, instead of the path created by the provided config.
var ignore_resource_path: bool


func _init(config: Dictionary = DEFAULT_CONFIG) -> void:
	for key in DEFAULT_CONFIG.keys(): self[key] = DEFAULT_CONFIG[key]
	for key in config.keys(): 
		if key in DEFAULT_CONFIG.keys(): 
			self[key] = config[key]
	if save_on_change and resource:
		resource.changed.connect(commit)


func commit() -> void:
	if not resource: return
	var path := get_path()
	if not DirAccess.dir_exists_absolute(path.get_base_dir()):
		DirAccess.make_dir_absolute(path.get_base_dir())
	ResourceSaver.save(resource, path)


func get_path() -> String:
	if has_resource_path():
		return resource.resource_path
	return get_dir().path_join(get_file())


func get_file() -> String:
	if has_resource_path():
		return resource.resource_path.get_file()
	return "{0}.{1}".format([get_filename(), get_extension()])


func get_filename() -> String:
	if has_resource_path():
		var name := resource.resource_path.get_file()
		return name.replace(name.get_extension(), "")
	if filename is Callable: return filename.call()
	if filename is String: return filename
	return ""


func get_extension() -> String:
	if has_resource_path():
		return resource.resource_path.get_extension()
	return "res" if binary else "tres"


func get_dir() -> String:
	if has_resource_path():
		return resource.resource_path.get_base_dir()
	if base_dir is Callable: return base_dir.call()
	return base_dir


func has_resource_path() -> bool:
	return not ignore_resource_path and resource and len(resource.resource_path)
