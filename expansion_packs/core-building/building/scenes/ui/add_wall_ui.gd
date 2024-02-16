extends VBoxContainer

@export var building: Building

var start = null
var start_handle = null
var end = null
var end_handle = null

@onready var viewport := get_viewport()
@onready var camera := viewport.get_camera_3d()
@onready var ui_root := UIConfig.get_game_mode_ui_root()
@onready var history := BuildModeState.get_history()


func _unhandled_input(event: InputEvent) -> void:
	if event is InputEventScreenDrag:
		viewport.set_input_as_handled()
		if start == null:
			start_handle = Vec3.project_position_to_floor(camera, get_global_mouse_position())
		elif end == null:
			end_handle = Vec3.project_position_to_floor(camera, get_global_mouse_position())
	
	if event.is_action_pressed("click"):
		viewport.set_input_as_handled()
		if start == null:
			start = Vec3.project_position_to_floor(camera, event.position)
			start_handle = start
		elif end == null:
			end = Vec3.project_position_to_floor(camera, event.position)
			end_handle = end
	
	if event.is_action_released("click") and start != null and end != null:
		viewport.set_input_as_handled()
		add_wall()


func add_wall() -> void:
	var positions = {
		start = Vec2.from(start),
		end = Vec2.from(end),
		start_handle = Vec2.from(start_handle - start),
		end_handle = Vec2.from(end_handle - end)
	}
	var index = building.wall_count
	if history: history.add(
		building,
		&"Add Wall",
		building.add_wall.bind(positions.start, positions.end, positions.start_handle, positions.end_handle),
		building.remove_wall.bind(index)
	)
	if ui_root: ui_root.pop()


func _on_cancel_button_pressed() -> void:
	if ui_root: ui_root.pop()
