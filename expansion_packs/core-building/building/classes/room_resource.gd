class_name RoomResource extends ModdableResource


@export var tiles: Array[Vector2i] = []:
	set(value):
		tiles = value
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


func get_interior_resource() -> ItemResource:
	return ItemDB.find_by_id(interior_id)


func get_exterior_resource() -> ItemResource:
	return ItemDB.find_by_id(exterior_id)


func get_floor_resource() -> ItemResource:
	return ItemDB.find_by_id(floor_id)


func get_size() -> Vector2i:
	if tiles.is_empty(): return Vector2i.ZERO
	var rect := Rect2i(tiles[0], Vector2i.ONE)
	for tile in tiles:
		rect = rect.expand(tile)
	return rect.size + Vector2i.ONE
