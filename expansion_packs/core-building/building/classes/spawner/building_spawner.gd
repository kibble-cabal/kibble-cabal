class_name BuildingSpawner extends Spawner


func _spawn(world: Node3D) -> Array[Node]:
	var building := resource as Building
	var node := MeshInstance3D.new()
	world.add_child(node)
	node.mesh = building.generate_mesh()
	return [node]


func _update(nodes: Array[Node]) -> void:	
	var building := resource as Building
	if not nodes.is_empty() and nodes[0] is MeshInstance3D:
		nodes[0].mesh = building.generate_mesh()
