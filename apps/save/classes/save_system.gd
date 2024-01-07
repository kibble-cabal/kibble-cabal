extends Node

var current_save: SaveResource


func _ready() -> void:
	var discovered_saves := discover_saves()
	if discovered_saves.size(): current_save = discovered_saves[len(discovered_saves) - 1]
	else: current_save = SaveResource.new()
	commit_changes()


func open_save(save_value: SaveResource) -> void:
	close_save()
	current_save = save_value


func close_save() -> void:
	commit_changes()
	current_save = null


func commit_changes() -> void:
	if current_save:
		current_save.commit_changes()
		print("Saving ", current_save.id, "...")


func discover_saves() -> Array[SaveResource]:
	var saves: Array[SaveResource] = []
	var base_dir := "user://saves"
	if not DirAccess.dir_exists_absolute(base_dir):
		return []
	for dir in DirAccess.get_directories_at(base_dir):
		var path := base_dir.path_join(dir.path_join("save.tres"))
		if FileAccess.file_exists(path):
			var resource := ResourceLoader.load(path, "SaveResource")
			if resource and resource is SaveResource: saves.append(resource)
	return saves


func lua_fields() -> Array[String]:
	return ["open_save", "close_save", "commit_changes", "current_save", "discover_saves"]
