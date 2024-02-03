class_name RoomPolygonUISpawner extends Spawner

## Spawns polygon editor for provided room.


func _spawn(world: Node3D) -> Array[Node]:
	var room := resource as RoomResource
	if room:
		var node := PolygonEditor3D.new()
		node.curve = room.polygon
		node.size = BuildingConfig.WallThickness * 1.5
		node.input_margin = BuildingConfig.WallThickness * 0.1
		node.dragging_modulate = Color("#6366f1")
		node.position = Vector3(room.origin.x, 0, room.origin.y)
		world.add_child(node)
		return [node]
	return []
