class_name RoomSpawner extends Spawner

const RoomScene := preload("../../scenes/room_scene.tscn")


func _spawn(world: Node3D) -> Array[Node]:
	var node := RoomScene.instantiate()
	var room := resource as RoomResource
	if room: node.room = room
	node.position = Vector3(room.origin.x, 0, room.origin.y)
	world.add_child(node)
	return [node]
