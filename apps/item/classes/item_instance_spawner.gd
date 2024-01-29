class_name ItemInstanceSpawner extends Spawner


func _spawn(world: Node3D) -> Array[Node]:
	if resource and resource is ItemInstanceResource:
		var node: Node3D = resource.instantiate_scene()
		node.position = resource.location
		world.add_child(node)
		return [node]
	return []
