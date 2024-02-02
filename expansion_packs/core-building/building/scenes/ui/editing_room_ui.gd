extends VBoxContainer

@export var building: BuildingResource
@export var room: RoomResource

var spawner: RoomSpawner

@onready var ui_root := UIConfig.get_game_mode_ui_root()


func _ready() -> void:
	spawner = RoomSpawner.new(room)
	room.changed.connect(respawn)
	respawn()


func respawn() -> void:
	if is_inside_tree():
		for child in $Spawner.get_children():
			child.queue_free()
		spawner.spawn($Spawner)


func _on_done_button_pressed() -> void:
	building.add_room(room)
	if ui_root: ui_root.pop()


func _on_cancel_button_pressed() -> void:
	if ui_root: ui_root.pop()
