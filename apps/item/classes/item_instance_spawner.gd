class_name ItemInstanceSpawner extends Spawner


func _spawn(world: Node2D) -> Array[Node]:
	if resource and resource is ItemInstanceResource:
		var node: Node2D = resource.instantiate_scene()
		node.position = resource.location
		world.add_child(node)
		return [node]
	return []
