extends Control3D

@export var building: Building


func _on_edit_button_pressed() -> void:
	building.EditRequested.emit()


func _on_destroy_button_pressed() -> void:
	building.DestroyRequested.emit()
