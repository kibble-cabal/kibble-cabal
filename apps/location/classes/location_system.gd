extends Node

signal location_entered(location: LocationResource)
signal location_exited(location: LocationResource)
signal location_changed

var current_location: LocationResource = null
var current_map: Node = null


var current_state: LocationStateResource:
	get: 
		if current_location: return current_location.get_or_create_state()
		return null


func enter(location: LocationResource) -> void:
	if current_location: exit()
	if location:
		current_location = location
		_spawn_map()
		_spawn_spawners()
		location_entered.emit(current_location)
		location_changed.emit()
		Log.from(self, "Entering location: " + current_location.name)


func exit() -> void:
	_despawn_spawners()
	_despawn_map()
	
	current_location = null
	current_map = null
	location_exited.emit(current_location)
	
	# If no location has been entered after processing, they're probably not entering a new location,
	# so we can emit [signal location_changed] without worrying too hard about it getting emitted twice
	await get_tree().process_frame
	if not current_location: location_changed.emit()


func get_world_root() -> Node3D:
	return get_tree().get_first_node_in_group(&"world_root") as Node3D


## Spawns the map for the current location.
func _spawn_map() -> void:
	if not current_location or not current_location.map: return
	current_map = current_location.map.instantiate()
	var world_root := get_world_root()
	if world_root:
		world_root.add_child(current_map)
		world_root.move_child(current_map, 0)


## Spawns all spawners for the current location.
func _spawn_spawners() -> void:
	var state := current_location.get_or_create_state() if current_location else null
	if not state or not current_map: return
	Sig.try_connect(state.spawners_changed, _on_spawners_changed)
	for spawner in state.spawners:
		spawner.spawn(current_map)


## Adds any new spawners, removes any outdates spawners.
func _on_spawners_changed() -> void:
	var state := current_location.get_or_create_state() if current_location else null
	if not state or not current_map: return
	# Add new spawners
	for spawner in state.spawners.filter(func(spawner): return not spawner.has_spawned):
		print("Spawning", spawner)
		spawner.spawn(current_map)
	# Remove spawners that no longer exist on the state.
	for node in get_tree().get_nodes_in_group(Spawner.GroupName):
		var spawner: Spawner = node.get_meta(Spawner.MetaName)
		if spawner and not spawner in state.spawners:
			spawner.despawn()


## Despawns the map for the current location.
func _despawn_map() -> void:
	if current_map: current_map.queue_free()


## Despawns all spawners for the current location.
func _despawn_spawners() -> void:
	var state := current_location.get_or_create_state() if current_location else null
	if not state or not current_map: return
	Sig.try_disconnect(state.spawners_changed, _on_spawners_changed)
	for spawner in state.spawners:
		spawner.despawn()


func lua_fields() -> Array:
	return ["enter", "exit", "current_location", "current_state", "current_map", "get_world_root"]


func _to_string() -> String:
	return "LocationSystem"
