class_name BuildingUISpawner extends Spawner


const Scene := preload("../../scenes/ui/building_hud.tscn")


func _spawn(world: Node3D) -> Array[Node]:
	var node := Scene.instantiate()
	var building := resource as BuildingResource
	var rect := building.get_rect()
	var center := rect.position + rect.size / 2
	node.building = building
	node.local_position = Vector3(center.x, BuildingConfig.WallHeight, center.y)
	world.add_child(node)
	return [node]
