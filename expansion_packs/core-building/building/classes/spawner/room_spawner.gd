class_name RoomSpawner extends Spawner

const RoomScene := preload("../../scenes/room_scene.tscn")


func _spawn(world: Node3D) -> Array[Node]:
	var node := RoomScene.instantiate()
	var room := resource as RoomResource
	if room: node.room = room
	world.add_child(node)
	return [node]
