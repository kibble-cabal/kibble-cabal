class_name BuildingSpawner extends Spawner


func _spawn(world: Node3D) -> Array[Node]:
	var building := resource as BuildingResource
	var node := Node3D.new()
	world.add_child(node)
	if building: for room in building.rooms:
		var spawner := RoomSpawner.new(room)
		spawner.spawn(node)
	return [node]
