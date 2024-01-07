extends Node

signal location_entered(location: LocationResource)
signal location_exited(location: LocationResource)

var current_location: LocationResource = null
var current_map: Node = null


func enter(location: LocationResource) -> void:
	if current_location: exit()
	if location:
		current_location = location
		if location.map:
			current_map = location.map.instantiate()
			get_tree().current_scene.add_child(current_map)
		location_entered.emit(current_location)
		prints("Entering location:", current_location.name)


func exit() -> void:
	if current_map:
		current_map.queue_free()
	current_location = null
	current_map = null
	location_exited.emit(current_location)
