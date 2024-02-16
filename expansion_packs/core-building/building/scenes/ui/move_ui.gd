extends VBoxContainer


@export var building: Building
@export var walls: PackedInt32Array
@export var floors: PackedInt32Array
@export var start_position: Vector3


@onready var ui_root := UIConfig.get_game_mode_ui_root()

@onready var history := BuildModeState.get_history()


func _on_cursor_clicked(pos: Vector3) -> void:
	var delta = Vec2.from(pos - start_position)
	if history:
		if not is_building_just_created():
			history.add_multi(
				building,
				&"Move",
				[building.move_walls_by.bind(walls, delta), building.move_floors_by.bind(floors, delta)],
				[building.move_walls_by.bind(walls, -delta), building.move_floors_by.bind(floors, -delta)],
			)
		try_create_building(delta)
	if ui_root: ui_root.pop()


func is_building_just_created() -> bool:
	return not LocationSystem.current_state.has_spawners_with_resource(building)


func try_create_building(delta: Vector2) -> void:
	if is_building_just_created():
		building.move_walls_by(delta)
		building.move_floors_by(delta)
		history.add(
			building,
			"Add Building",
			LocationSystem.current_state.add_spawner.bind(BuildingSpawner.new(building)),
			LocationSystem.current_state.remove_spawners_with_resource.bind(building)
		)


func _on_cancel_button_pressed() -> void:
	if ui_root: ui_root.pop()
