class_name BuildingUISpawner extends Spawner


const Scene := preload("../../scenes/ui/building_hud.tscn")


func _spawn(world: Node3D) -> Array[Node]:
	var node := Scene.instantiate()
	node.building = resource as Building
	world.add_child(node)
	return [node]


func _update(nodes: Array[Node]) -> void:
	if not nodes.is_empty():
		var node := nodes[0]
		var building := resource as Building
		if node and building:
			var center: Vector2 = building.get_centroid()
			node.building = building
			node.local_position = Vector3(center.x, 1.0, center.y)
