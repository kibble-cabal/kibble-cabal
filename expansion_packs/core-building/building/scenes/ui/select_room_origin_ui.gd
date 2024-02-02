extends VBoxContainer


const EditingRoomUI := preload("editing_room_ui.tscn")


@export var building: BuildingResource
@export var room: RoomResource

@onready var ui_root := UIConfig.get_game_mode_ui_root()


func _on_cursor_clicked(pos: Vector3) -> void:
	room.origin = Vector2(pos.x, pos.z)
	var scene := EditingRoomUI.instantiate()
	scene.room = room
	scene.building = building
	if ui_root: 
		ui_root.pop() # remove this UI from the stack, we won't ever go back here
		ui_root.push(scene)
	

func _on_cancel_button_pressed() -> void:
	if ui_root: ui_root.pop()
