extends VBoxContainer

@export var building: BuildingResource
@export var room: RoomResource

@onready var ui_root := UIConfig.get_game_mode_ui_root()
@onready var spawner := RoomPolygonUISpawner.new(room)


func _ready() -> void:
	$DesignRoomUI.room = room
	$DesignRoomUI.update()
	spawner.spawn($Spawner)
	BuildModeState.get_history().on_after_undo(&"Add Room", _on_undo_add_room)


func _on_done_button_pressed() -> void:
	if ui_root: ui_root.pop()


func _on_undo_add_room() -> void:
	if ui_root: ui_root.pop()
