class_name WallUISpawner extends Spawner

## Spawns editor for provided wall.

const WallHUDScene := preload("../../scenes/ui/wall_hud.tscn")

@export var index: int


func _spawn(world: Node3D) -> Array[Node]:
	var building := resource as Building
	if building and get_wall(): return [
		make_polygon_editor(world), 
		make_hud(world)
	]
	return []


func _update(nodes: Array[Node]) -> void:
	if nodes.size() >= 2:
		var node := nodes[0] as PolygonEditor3D
		var building := resource as Building
		var wall := get_wall()
		if node and building and wall:
			node.size = wall.thickness * 2
			node.input_margin = wall.thickness * 0.1


func make_polygon_editor(world: Node3D) -> PolygonEditor3D:
	var wall := get_wall()
	var node := PolygonEditor3D.new()
	node.point_changed.connect(_on_point_changed)
	node.history = BuildModeState.get_history()
	node.polygon = PackedVector2Array([wall.start, wall.end])
	node.modulate = Color("#818cf8")
	node.dragging_modulate = Color("#6366f1")
	node.custom_snap_method = func(position: Vector3) -> Vector3:
		if resource is Building: return Vec3.from(resource.snap(Vec2.from(position), BuildingConfig.SnapThreshold))
		return position
	world.add_child(node)
	return node


func make_hud(world: Node3D) -> Node:
	var wall := get_wall()
	var node := WallHUDScene.instantiate()
	node.building = resource as Building
	node.index = index
	world.add_child(node)
	return node


func get_wall() -> WallRef:
	if resource is Building:
		return resource.get_wall(index)
	return null


func _on_point_changed(i: int, pos: Vector2) -> void:
	var wall := get_wall()
	var building := resource as Building
	if wall and building: match i:
		0: wall.start = pos
		1: wall.end = pos
