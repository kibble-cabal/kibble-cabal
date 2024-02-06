class_name RoomSpawner extends Spawner

const RoomScene := preload("../../scenes/room_scene.tscn")


func _spawn(world: Node3D) -> Array[Node]:
	var node := RoomScene.instantiate()
	var room := resource as RoomResource
	if room: node.room = room
	world.add_child(node)
	return [node]


func _update(nodes: Array[Node]) -> void:
	if not nodes.is_empty():
		var node := nodes[0] as Node3D
		var room := resource as RoomResource
		if node and room:
			node.position = Vector3(room.origin.x, 0, room.origin.y)
