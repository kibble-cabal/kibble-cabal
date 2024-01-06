extends Node


var current_location: LocationResource = null
var current_map: Node = null


func enter(location: LocationResource) -> void:
	if current_location: exit()
	current_location = location
	current_map = location.map.instantiate()
	get_tree().current_scene.add_child(current_map)


func exit() -> void:
	if current_map:
		current_map.queue_free()
	current_location = null
	current_map = null
