extends Node

signal location_entered(location: LocationResource)
signal location_exited(location: LocationResource)
signal location_changed

var current_location: LocationResource = null
var current_map: Node = null


var current_state: LocationStateResource:
	get:
		if SaveSystem.current_save: 
			return SaveSystem.current_save.get_or_create_location_state(current_location.name)
		return null


func enter(location: LocationResource) -> void:
	if current_location: exit()
	if location:
		current_location = location
		if location.map:
			current_map = location.map.instantiate()
			var world_root := get_tree().get_first_node_in_group("world_root")
			if world_root:
				world_root.add_child(current_map)
				world_root.move_child(current_map, 0)
				_spawn_inventory()
		location_entered.emit(current_location)
		location_changed.emit()
		Log.from(self, "Entering location: " + current_location.name)


func exit() -> void:
	if current_map:
		current_map.queue_free()
	current_location = null
	current_map = null
	location_exited.emit(current_location)
	
	# If no location has been entered after processing, they're probably not entering a new location,
	# so we can emit [signal location_changed] without worrying too hard about it getting emitted twice
	await get_tree().process_frame
	if not current_location: location_changed.emit()


func _spawn_inventory() -> void:
	if not current_location or not current_map or not SaveSystem.current_save: return
	var state := current_state
	if not state.inventory: state.inventory = InventoryResource.new()
	for item_instance in state.inventory.item_instances:
		var node: Node2D = item_instance.instantiate_scene()
		node.position = item_instance.location
		current_map.add_child(node)


func lua_fields() -> Array:
	return ["enter", "exit", "current_location"]


func _to_string() -> String:
	return "LocationSystem"
