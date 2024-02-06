class_name RoomUISpawner extends Spawner

const Scene := preload("../../scenes/ui/room_hud.tscn")


func _spawn(world: Node3D) -> Array[Node]:
	var node := Scene.instantiate()
	node.room = resource as RoomResource
	world.add_child(node)
	return [node]


func _update(nodes: Array[Node]) -> void:
	if not nodes.is_empty():
		var node := nodes[0]
		var room := resource as RoomResource
		if node and room:
			var center := room.get_center()
			print(center)
			node.local_position = Vector3(center.x, BuildingConfig.WallHeight, center.y)
