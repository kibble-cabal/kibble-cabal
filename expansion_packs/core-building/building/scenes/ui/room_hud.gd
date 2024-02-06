extends Control3D

@export var room: RoomResource


func _on_edit_button_pressed() -> void:
	room.edit_requested.emit()


func _on_move_button_pressed() -> void:
	room.move_requested.emit()


func _on_destroy_button_pressed() -> void:
	room.destroy_requested.emit()
