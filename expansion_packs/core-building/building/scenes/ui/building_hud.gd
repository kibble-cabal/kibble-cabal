extends Control3D

@export var building: BuildingResource


func _on_edit_button_pressed() -> void:
	building.edit_requested.emit()


func _on_destroy_button_pressed() -> void:
	building.destroy_requested.emit()
