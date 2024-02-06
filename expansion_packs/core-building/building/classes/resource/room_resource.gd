class_name RoomResource extends ModdableResource

signal edit_requested
signal move_requested
signal destroy_requested


@export var polygon: Curve2D = Curve2D.new():
	set(value):
		Sig.switch_connection(polygon, value, &"changed", emit_changed)
		polygon = value
		emit_changed()

@export var origin: Vector2:
	set(value):
		origin = value
		emit_changed()

@export_category("Design")

@export var interior_id: StringName:
	set(value):
		interior_id = value
		emit_changed()

@export var exterior_id: StringName:
	set(value):
		exterior_id = value
		emit_changed()

@export var floor_id: StringName:
	set(value):
		floor_id = value
		emit_changed()


# Additional properties I may add later:
# @export var interior_trim_id: StringName
# @export var exterior_trim_id: StringName
# @export var level: int = 0


func add_point(point: Vector2) -> void:
	polygon.add_point(point)
	emit_changed()


func set_origin(value: Vector2) -> void:
	origin = value


func get_interior_resource() -> ItemResource:
	return ItemDB.find_by_id(interior_id)


func get_exterior_resource() -> ItemResource:
	return ItemDB.find_by_id(exterior_id)


func get_floor_resource() -> ItemResource:
	return ItemDB.find_by_id(floor_id)


func get_size() -> Vector2:
	var points := polygon.tessellate(3)
	if points.is_empty(): return Vector2.ZERO
	var rect := Rect2(points[0], Vector2.ZERO)
	for point in points:
		rect = rect.expand(point)
	return rect.size


func get_rect() -> Rect2:
	return Rect2(origin, get_size())


func get_center() -> Vector2:
	var rect := get_rect()
	return rect.position + rect.size / 2


func get_mesh() -> ProceduralRoomMesh:
	var points := polygon.tessellate(3)
	var mesh := ProceduralRoomMesh.new()
	mesh.points = points
	mesh.material_wall_exterior = create_material_from_item(get_exterior_resource())
	mesh.material_wall_interior = create_material_from_item(get_interior_resource())
	mesh.material_floor_top = create_material_from_item(get_floor_resource())
	mesh.wall_height = BuildingConfig.WallHeight
	mesh.wall_thickness = BuildingConfig.WallThickness
	mesh.floor_thickness = BuildingConfig.FloorThickness
	mesh.generate()
	return mesh


func create_material_from_item(item: ItemResource) -> BaseMaterial3D:
	var mat := StandardMaterial3D.new()
	if item and item.physics_resource and item.physics_resource.static_image:
		mat.albedo_texture = item.physics_resource.static_image
		mat.uv1_triplanar = true
		mat.uv1_scale.y = 1.0 / BuildingConfig.WallHeight
	return mat
