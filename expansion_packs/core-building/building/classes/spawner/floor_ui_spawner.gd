class_name FloorUISpawner extends Spawner

## Spawns polygon editor for provided floor.

@export var index: int


func _spawn(world: Node3D) -> Array[Node]:
	var building := resource as Building
	if building:
		var node := CurveEditor3D.new()
		if GameModeSystem.current_state is BuildModeState:
			node.history = GameModeSystem.current_state.history
		node.enable_add_points = true
		node.enable_remove_points = true
		node.curve = building.get_floor_polygon(index)
		node.modulate = Color("#818cf8")
		node.dragging_modulate = Color("#6366f1")
		world.add_child(node)
		return [node]
	return []


func _update(nodes: Array[Node]) -> void:
	if not nodes.is_empty():
		var node := nodes[0] as PolygonEditor3D
		var building := resource as Building
		if node and building:
			var floor_ref: FloorRef = building.get_floor(index)
			node.size = floor_ref.thickness
			node.input_margin = floor_ref.thickness * 0.1
