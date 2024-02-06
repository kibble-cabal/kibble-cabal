class_name RoomPolygonUISpawner extends Spawner

## Spawns polygon editor for provided room.


func _spawn(world: Node3D) -> Array[Node]:
	var room := resource as RoomResource
	if room:
		var node := PolygonEditor3D.new()
		if GameModeSystem.current_state is BuildModeState:
			node.history = GameModeSystem.current_state.history
		node.enable_add_points = true
		node.enable_remove_points = true
		node.curve = room.polygon
		node.modulate = Color("#818cf8")
		node.dragging_modulate = Color("#6366f1")
		world.add_child(node)
		return [node]
	return []


func _update(nodes: Array[Node]) -> void:
	if not nodes.is_empty():
		var node := nodes[0] as PolygonEditor3D
		var room := resource as RoomResource
		if node and room:
			node.position = Vector3(room.origin.x, 0, room.origin.y)
			node.size = BuildingConfig.WallThickness * 1.5
			node.input_margin = BuildingConfig.WallThickness * 0.1
