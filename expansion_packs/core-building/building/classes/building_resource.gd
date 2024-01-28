class_name BuildingResource extends ModdableResource


@export var rooms: Array[RoomResource] = []:
	set(value):
		rooms = value
		emit_changed()

@export_category("Design")

@export var roof_id: StringName:
	set(value):
		roof_id = value
		emit_changed()


func get_roof_resource() -> ItemResource:
	return ItemDB.find_by_id(roof_id)
