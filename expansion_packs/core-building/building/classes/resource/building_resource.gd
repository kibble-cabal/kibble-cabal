class_name BuildingResource extends ModdableResource

signal destroy_requested
signal edit_requested

## [br][b]Note:[/b] Should not be mutated directly. Use [method add_room] or [method remove_room] instead.
@export var rooms: Array[RoomResource] = []:
	set(value):
		rooms = value
		emit_changed()

@export_category("Design")

@export var roof_id: StringName:
	set(value):
		roof_id = value
		emit_changed()


func add_room(room: RoomResource) -> void:
	if not room in rooms:
		rooms.append(room)
		emit_changed()


func remove_room(room: RoomResource) -> void:
	if room in rooms:
		rooms.erase(room)
		emit_changed()


func get_roof_resource() -> ItemResource:
	return ItemDB.find_by_id(roof_id)


func get_rect() -> Rect2:
	var rect := Rect2()
	for room in rooms:
		rect = rect.merge(room.get_rect())
	return rect


func get_center() -> Vector2:
	var rect := get_rect()
	return rect.position + rect.size / 2
