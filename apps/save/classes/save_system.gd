extends Node

var current_save: SaveResource


func _ready() -> void:
	open_save(SaveResource.new())
	commit_changes()


func open_save(save_value: SaveResource) -> void:
	close_save()
	current_save = save_value


func close_save() -> void:
	commit_changes()
	current_save = null


func commit_changes() -> void:
	if current_save: current_save.commit_changes()
