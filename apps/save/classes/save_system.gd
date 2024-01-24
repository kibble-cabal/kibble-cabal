extends Node

signal save_opened(save: SaveResource)
signal save_closed(save: SaveResource)
signal before_saved
signal saved

var timer := Timer.new()
var current_save: SaveResource
var session_start_time: float = 0


func _ready() -> void:
	timer.autostart = true
	timer.wait_time = 5.0
	timer.timeout.connect(commit_changes)
	add_child(timer)
	var discovered_saves := discover_saves()
	if discovered_saves.size(): open_save(discovered_saves[len(discovered_saves) - 1])
	else: current_save = SaveResource.new()
	commit_changes()


func open_save(save_value: SaveResource) -> void:
	close_save()
	current_save = save_value
	if current_save:
		Log.from(self, "Opening " + current_save.id)
		session_start_time = Time.get_unix_time_from_system()
		save_opened.emit(current_save)
		_connect_current_save()


func close_save() -> void:
	if current_save:
		commit_changes()
		var prev_save := current_save
		current_save = null
		save_closed.emit(prev_save)
		_disconnect_current_save()


func commit_changes() -> void:
	if current_save: current_save.commit_changes()


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


func get_setting(key: String, default_value):
	if current_save and current_save.settings:
		return current_save.settings.settings.get(key, default_value)
	return default_value


func set_setting(key: String, value) -> void:
	if current_save and current_save.settings:
		current_save.settings.set_setting(key, value)


func lua_fields() -> Array:
	return ["open_save", "close_save", "commit_changes", "current_save", "discover_saves", "get_setting", "set_setting"]


func _to_string() -> String:
	return "SaveSystem"


func _connect_current_save() -> void:
	if current_save:
		Sig.try_connect(current_save.before_saved, _on_before_saved)
		Sig.try_connect(current_save.saved, _on_saved)


func _disconnect_current_save() -> void:
	if current_save:
		Sig.try_disconnect(current_save.before_saved, _on_before_saved)
		Sig.try_disconnect(current_save.saved, _on_saved)


func _on_before_saved() -> void:
	if current_save:
		var now := Time.get_unix_time_from_system()
		var session_length := now - session_start_time
		current_save.time_played += session_length
		session_start_time = now
		Log.from(self, "Saving {0} (played for {1}s)".format([current_save.id, roundf(session_length)]))
		before_saved.emit()


func _on_saved() -> void:
	if current_save: saved.emit()
