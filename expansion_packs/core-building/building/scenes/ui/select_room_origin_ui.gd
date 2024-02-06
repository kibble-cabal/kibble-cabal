extends VBoxContainer


@export var building: BuildingResource
@export var room: RoomResource

@onready var ui_root := UIConfig.get_game_mode_ui_root()

@onready var history := BuildModeState.get_history()


func _on_cursor_clicked(pos: Vector3) -> void:
	if history:
		if not is_building_just_created() and not is_room_just_created():
			history.add(
				room,
				"Move Room",
				room.set_origin.bind(Vector2(pos.x, pos.z)),
				room.set_origin.bind(room.origin)
			)
		try_create_building()
		try_create_room(pos)
	if ui_root: ui_root.pop()


func is_building_just_created() -> bool:
	return not LocationSystem.current_state.has_spawners_with_resource(building)


func is_room_just_created() -> bool:
	return not building.has_room(room)


func try_create_building() -> void:
	if is_building_just_created():
		history.add(
			building,
			"Add Building",
			LocationSystem.current_state.add_spawner.bind(BuildingSpawner.new(building)),
			LocationSystem.current_state.remove_spawners_with_resource.bind(building)
		)


func try_create_room(pos: Vector3) -> void:
	if is_room_just_created():
		room.origin = Vector2(pos.x, pos.z)
		history.add(
			building,
			"Add Room",
			building.add_room.bind(room),
			building.remove_room.bind(room)
		)


func _on_cancel_button_pressed() -> void:
	if ui_root: ui_root.pop()
