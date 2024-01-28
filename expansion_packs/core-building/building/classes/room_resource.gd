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


func get_interior_resource() -> ItemResource:
	return ItemDB.find_by_id(interior_id)


func get_exterior_resource() -> ItemResource:
	return ItemDB.find_by_id(exterior_id)


func get_floor_resource() -> ItemResource:
	return ItemDB.find_by_id(floor_id)
