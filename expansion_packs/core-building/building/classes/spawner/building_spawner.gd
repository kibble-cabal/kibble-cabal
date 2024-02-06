class_name BuildingSpawner extends Spawner


## [Dictionary][[RoomResource], [RoomSpawner]]
var room_spawners: Dictionary = {}


func _spawn(world: Node3D) -> Array[Node]:
	var building := resource as BuildingResource
	var node := Node3D.new()
	world.add_child(node)
	if building: 
		for room in building.rooms: _spawn_room(node, room)
	return [node]


func _update(nodes: Array[Node]) -> void:	
	var building := resource as BuildingResource
	
	for room: RoomResource in room_spawners.keys():
		if not building.has_room(room):
			_despawn_room(room)
		else: 
			(room_spawners[room] as RoomSpawner).update()
	
	for room in building.rooms:
		if room not in room_spawners:
			_spawn_room(nodes[0], room)


func _despawn(nodes: Array[Node]) -> void:
	super(nodes)
	room_spawners.clear()


func _spawn_room(node: Node, room: RoomResource) -> void:
	if not room in room_spawners:
		var spawner := RoomSpawner.new(room)
		room_spawners[room] = spawner
		spawner.spawn(node)


func _despawn_room(room: RoomResource) -> void:
	var spawner := room_spawners[room] as RoomSpawner
	if spawner: spawner.despawn()
	room_spawners.erase(room)
