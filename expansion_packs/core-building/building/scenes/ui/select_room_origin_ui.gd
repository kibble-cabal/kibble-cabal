extends VBoxContainer


@export var building: BuildingResource
@export var room: RoomResource

@onready var ui_root := UIConfig.get_game_mode_ui_root()


func _on_cursor_clicked(pos: Vector3) -> void:
	room.origin = Vector2(pos.x, pos.z)
	if ui_root: ui_root.pop()
	

func _on_cancel_button_pressed() -> void:
	if ui_root: ui_root.pop()
