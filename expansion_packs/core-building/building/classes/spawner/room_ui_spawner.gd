class_name RoomUISpawner extends Spawner

const Scene := preload("../../scenes/ui/room_hud.tscn")


func _spawn(world: Node3D) -> Array[Node]:
	var room := resource as RoomResource
	var center := room.get_center()
	var node := Scene.instantiate()
	node.room = room
	node.local_position = Vector3(center.x, BuildingConfig.WallHeight, center.y)
	world.add_child(node)
	return [node]
