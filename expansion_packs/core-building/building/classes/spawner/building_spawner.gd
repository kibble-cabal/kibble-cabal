class_name BuildingSpawner extends Spawner

var room_spawners: Array[RoomSpawner] = []


func _spawn(world: Node3D) -> Array[Node]:
	var building := resource as BuildingResource
	var node := Node3D.new()
	world.add_child(node)
	if building: for room in building.rooms:
		var spawner := RoomSpawner.new(room)
		spawner.spawn(node)
		room_spawners.append(spawner)
	return [node]


func _update(_nodes: Array[Node]) -> void:
	for spawner in room_spawners:
		spawner.update()
